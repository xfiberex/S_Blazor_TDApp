using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(DbTdappContext context) : ControllerBase
    {
        private readonly DbTdappContext _context = context;

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<UsuarioDTO>>();
            var listaUsuarioDTO = new List<UsuarioDTO>();

            try
            {
                var usuarios = await _context.Usuarios
                                             .Include(u => u.IdRolNavigation)
                                             .ToListAsync();
                foreach (var item in usuarios)
                {
                    listaUsuarioDTO.Add(new UsuarioDTO
                    {
                        UsuarioId = item.UsuarioId,
                        NombreUsuario = item.NombreUsuario,
                        Email = item.Email,
                        RolId = item.RolId,
                        Activo = item.Activo,
                        FechaCreacion = item.FechaCreacion,
                        FechaActualizacion = item.FechaActualizacion,
                        // Por seguridad, no se retorna la contraseña
                        Rol = new RolDTO
                        {
                            RolId = item.IdRolNavigation.RolId,
                            NombreRol = item.IdRolNavigation.NombreRol
                            // Se pueden mapear otros campos del rol si es necesario
                        }
                    });
                }

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
                                                  .FirstOrDefaultAsync(u => u.UsuarioId == id);
                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El usuario no existe.";
                    return NotFound(responseApi);
                }

                var usuarioDTO = new UsuarioDTO
                {
                    UsuarioId = usuarioEntity.UsuarioId,
                    NombreUsuario = usuarioEntity.NombreUsuario,
                    Clave = usuarioEntity.Clave,
                    Email = usuarioEntity.Email,
                    RolId = usuarioEntity.RolId,
                    Activo = usuarioEntity.Activo,
                    FechaCreacion = usuarioEntity.FechaCreacion,
                    FechaActualizacion = usuarioEntity.FechaActualizacion,
                    // No se retorna la contraseña por seguridad
                    Rol = new RolDTO
                    {
                        RolId = usuarioEntity.IdRolNavigation.RolId,
                        NombreRol = usuarioEntity.IdRolNavigation.NombreRol
                    }
                };

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
        public async Task<IActionResult> Guardar(UsuarioDTO usuario)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Se recomienda hashear la contraseña antes de almacenarla
                var usuarioEntity = new Usuario
                {
                    NombreUsuario = usuario.NombreUsuario,
                    Clave = usuario.Clave,
                    Email = usuario.Email,
                    RolId = usuario.RolId,
                    Activo = usuario.Activo,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = null
                };

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
        public async Task<IActionResult> Editar(UsuarioDTO usuario, int id)
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

                // Actualización de campos; la contraseña se actualiza solo si se desea
                usuarioEntity.NombreUsuario = usuario.NombreUsuario;
                usuarioEntity.Clave = usuario.Clave;
                usuarioEntity.Email = usuario.Email;
                usuarioEntity.RolId = usuario.RolId;
                usuarioEntity.Activo = usuario.Activo;
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
    }
}