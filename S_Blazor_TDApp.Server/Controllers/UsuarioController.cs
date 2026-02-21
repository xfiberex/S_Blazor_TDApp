using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Server.Utilities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuarioController(DbTdappContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var responseApi = new ResponseAPI<InicioSesionDTO>();

            try
            {
                // Busca al usuario en la base de datos por correo (sin comparar la contraseña en la consulta)
                var usuarioEntity = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .FirstOrDefaultAsync(u => u.Email == login.Email);

                // Verifica que el usuario exista y que la contraseña sea válida
                if (usuarioEntity == null || !PasswordHelper.VerifyPassword(login.Clave, usuarioEntity.Clave))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Credenciales inválidas.";
                    return Unauthorized(responseApi);
                }

                if (!usuarioEntity.Activo)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Usuario inactivo, contacte al administrador.";
                    return Unauthorized(responseApi);
                }

                var rolNombre = usuarioEntity.IdRolNavigation != null ? usuarioEntity.IdRolNavigation.NombreRol : "Sin Rol";

                // Generar Token JWT
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioEntity.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, usuarioEntity.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuarioEntity.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, rolNombre)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var inicioSesion = new InicioSesionDTO
                {
                    Nombre = usuarioEntity.NombreUsuario,
                    Correo = usuarioEntity.Email ?? string.Empty,
                    Rol = rolNombre,
                    Token = tokenString
                };

                responseApi.EsCorrecto = true;
                responseApi.Valor = inicioSesion;
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<UsuarioDTO>>();

            try
            {
                var usuarios = await _context.Usuarios
                                             .Include(u => u.IdRolNavigation)
                                             .AsNoTracking()
                                             .ToListAsync();

                // Mapea la lista de entidades a una lista de DTOs
                var listaUsuarioDTO = _mapper.Map<List<UsuarioDTO>>(usuarios);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaUsuarioDTO;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpGet]
        [Route("Buscar/{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var responseApi = new ResponseAPI<UsuarioDTO>();

            try
            {
                var usuarioEntity = await _context.Usuarios
                                                  .Include(u => u.IdRolNavigation)
                                                  .AsNoTracking()
                                                  .FirstOrDefaultAsync(u => u.UsuarioId == id);
                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El usuario no existe.";
                    return NotFound(responseApi);
                }

                var usuarioDTO = _mapper.Map<UsuarioDTO>(usuarioEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = usuarioDTO;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpGet]
        [Route("ExisteCodigo/{codigo}")]
        public async Task<IActionResult> ExisteCodigo(string codigo)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                // Se verifica si existe algún usuario con el código proporcionado.
                bool existe = await _context.Usuarios.AnyAsync(u => u.Codigo == codigo);
                responseApi.EsCorrecto = true;
                responseApi.Valor = existe;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }

            return Ok(responseApi);
        }

        [HttpGet]
        [Route("ObtenerPorEmail/{email}")]
        public async Task<IActionResult> ObtenerPorEmail(string email)
        {
            var responseApi = new ResponseAPI<UsuarioDTO>();

            try
            {
                var usuarioEntity = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El usuario no existe.";
                    return NotFound(responseApi);
                }

                var usuarioDTO = _mapper.Map<UsuarioDTO>(usuarioEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = usuarioDTO;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }

            return Ok(responseApi);
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(UsuarioDTO usuarioDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Validación: Verifica si ya existe un usuario con el mismo nombre de usuario
                bool existeNombre = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario == usuarioDTO.NombreUsuario);
                if (existeNombre)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El nombre de usuario ya existe.";
                    return BadRequest(responseApi);
                }

                // Validación: Verifica si ya existe un usuario con el mismo correo electrónico
                bool existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == usuarioDTO.Email);
                if (existeEmail)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El correo electrónico ya existe.";
                    return BadRequest(responseApi);
                }

                // Mapea el DTO a la entidad utilizando AutoMapper
                var usuarioEntity = _mapper.Map<Usuario>(usuarioDTO);

                // Hashea la contraseña antes de guardar
                if (!string.IsNullOrEmpty(usuarioEntity.Clave))
                {
                    usuarioEntity.Clave = PasswordHelper.HashPassword(usuarioEntity.Clave);
                }
                else
                {
                    throw new Exception("La contraseña es obligatoria.");
                }

                // Asigna la fecha de creación y deja nula la de actualización al guardar
                usuarioEntity.FechaCreacion = DateTime.Now;
                usuarioEntity.FechaActualizacion = null;

                _context.Usuarios.Add(usuarioEntity);
                await _context.SaveChangesAsync();

                if (usuarioEntity.UsuarioId != 0)
                {
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = usuarioEntity.UsuarioId;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se pudo guardar el usuario.";
                }
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPut]
        [Route("Editar/{id}")]
        public async Task<IActionResult> Editar(UsuarioDTO usuarioDTO, int id)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var usuarioEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El usuario no existe.";
                    return NotFound(responseApi);
                }

                // Validación: verificar si el nombre de usuario ya existe en otro registro
                bool existeNombre = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario == usuarioDTO.NombreUsuario && u.UsuarioId != id);
                if (existeNombre)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El nombre de usuario ya existe.";
                    return BadRequest(responseApi);
                }

                // Validación: verificar si el correo electrónico ya existe en otro registro
                bool existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == usuarioDTO.Email && u.UsuarioId != id);
                if (existeEmail)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El correo electrónico ya existe.";
                    return BadRequest(responseApi);
                }

                // Guardar la clave actual antes de mapear
                var claveActual = usuarioEntity.Clave;

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(usuarioDTO, usuarioEntity);

                // Restaurar la clave actual para no sobreescribirla
                usuarioEntity.Clave = claveActual;

                // Asigna la fecha de actualización
                usuarioEntity.FechaActualizacion = DateTime.Now;

                _context.Usuarios.Update(usuarioEntity);
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = usuarioEntity.UsuarioId;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var usuarioEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El usuario no existe.";
                    return NotFound(responseApi);
                }

                _context.Usuarios.Remove(usuarioEntity);
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPut]
        [Route("CambiarClave/{id}")]
        public async Task<IActionResult> CambiarClave(int id, [FromBody] CambioClaveDTO cambioClaveDto)
        {
            if (id != cambioClaveDto.UsuarioId)
            {
                return BadRequest(new { Message = "El ID del usuario no coincide." });
            }

            var usuarioEntity = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuarioEntity == null)
            {
                return NotFound(new { Message = "Usuario no encontrado." });
            }

            // Aquí se puede validar adicionalmente si se requiere algún control extra
            // ya que el DTO ya tiene validaciones (Compare) para asegurar que las contraseñas coincidan

            // Hashea la nueva contraseña usando el helper
            usuarioEntity.Clave = PasswordHelper.HashPassword(cambioClaveDto.NuevaClave);
            usuarioEntity.FechaActualizacion = DateTime.Now;

            _context.Usuarios.Update(usuarioEntity);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Contraseña actualizada correctamente." });
        }
    }
}