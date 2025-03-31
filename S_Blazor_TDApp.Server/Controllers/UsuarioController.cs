﻿using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UsuarioController(DbTdappContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Shared.LoginRequest request)
        {
            var responseApi = new ResponseAPI<LoginResponse>();

            try
            {
                // Buscar al usuario por nombre (o email, según convenga)
                var usuarioEntity = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == request.Username);

                if (usuarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Usuario no encontrado.";
                    return NotFound(responseApi);
                }

                // Validar la contraseña (en producción se debe trabajar con contraseñas hasheadas)
                if (usuarioEntity.Clave != request.Password)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Contraseña incorrecta.";
                    return Unauthorized(responseApi);
                }

                // Generar token JWT
                var token = GenerateJwtToken(usuarioEntity);

                var loginResponse = new LoginResponse
                {
                    Token = token,
                    Usuario = _mapper.Map<UsuarioDTO>(usuarioEntity)
                };

                responseApi.EsCorrecto = true;
                responseApi.Valor = loginResponse;
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

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(UsuarioDTO usuarioDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var usuarioEntity = _mapper.Map<Usuario>(usuarioDTO);

                // Asigna la fecha de creación y sin la de actualización al guardar
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

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(usuarioDTO, usuarioEntity);

                // Asignamos la fecha de actualización solo al editar
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

        // Token de autenticación
        private string GenerateJwtToken(Usuario usuario)
        {
            // Obtener la configuración de JWT desde appsettings.json
            var secret = _configuration["JWT:Secret"];
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var tokenValidity = Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]);

            var key = Encoding.ASCII.GetBytes(secret!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.IdRolNavigation.NombreRol)
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenValidity),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}