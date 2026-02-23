using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IEmailService _emailService;

        public UsuarioController(DbTdappContext context, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [EnableRateLimiting("LoginPolicy")]
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

                if (!usuarioEntity.CorreoConfirmado)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Debe confirmar su correo electrónico antes de iniciar sesión.";
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

                // Generar Refresh Token
                var refreshToken = GenerarRefreshToken();
                usuarioEntity.RefreshToken = refreshToken;
                usuarioEntity.RefreshTokenExpiracion = DateTime.UtcNow.AddDays(7); // Refresh token válido por 7 días
                await _context.SaveChangesAsync();

                var inicioSesion = new InicioSesionDTO
                {
                    Nombre = usuarioEntity.NombreUsuario,
                    Correo = usuarioEntity.Email ?? string.Empty,
                    Rol = rolNombre,
                    RolId = usuarioEntity.RolId,
                    Token = tokenString,
                    RefreshToken = refreshToken
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

        [HttpPost]
        [Route("Registro")]
        [AllowAnonymous]
        public async Task<IActionResult> Registro([FromBody] RegistroUsuarioDTO registro)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                if (await _context.Usuarios.AnyAsync(u => u.Email == registro.Email))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El correo electrónico ya está registrado.";
                    return BadRequest(responseApi);
                }

                if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == registro.NombreUsuario))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El nombre de usuario ya está en uso.";
                    return BadRequest(responseApi);
                }

                // Generar código único
                string nuevoCodigo;
                do
                {
                    nuevoCodigo = new Random().Next(10000, 99999).ToString();
                } while (await _context.Usuarios.AnyAsync(u => u.Codigo == nuevoCodigo));

                var tokenConfirmacion = Guid.NewGuid().ToString();

                var nuevoUsuario = new Usuario
                {
                    Codigo = nuevoCodigo,
                    NombreUsuario = registro.NombreUsuario,
                    NombreCompleto = registro.NombreCompleto,
                    Email = registro.Email,
                    Clave = PasswordHelper.HashPassword(registro.Clave),
                    RolId = 3, // Rol Empleado por defecto
                    Activo = true,
                    CorreoConfirmado = false,
                    TokenConfirmacion = tokenConfirmacion,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Enviar correo de confirmación
                var urlConfirmacion = $"https://localhost:7041/confirmar-correo?token={tokenConfirmacion}&email={registro.Email}";
                var mensaje = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirmación de Correo</title>
</head>
<body style='margin: 0; padding: 0; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f3f4f6; color: #333333;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f3f4f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); overflow: hidden; max-width: 600px; margin: 0 auto;'>
                    <tr>
                        <td align='center' style='background: linear-gradient(135deg, #6366f1, #8b5cf6); padding: 30px 20px;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: -0.5px;'>Task Management</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='margin-top: 0; color: #1f2937; font-size: 20px;'>¡Bienvenido, {registro.NombreCompleto}!</h2>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 25px;'>
                                Gracias por registrarte en Task Management. Para comenzar a utilizar tu cuenta y gestionar tus tareas, por favor confirma tu dirección de correo electrónico haciendo clic en el botón de abajo.
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center'>
                                        <a href='{urlConfirmacion}' style='display: inline-block; background: linear-gradient(135deg, #6366f1, #8b5cf6); color: #ffffff; text-decoration: none; padding: 14px 30px; border-radius: 8px; font-weight: 600; font-size: 16px; box-shadow: 0 4px 10px rgba(99,102,241,0.3);'>Confirmar mi correo</a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 30px;'>
                                Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:<br>
                                <a href='{urlConfirmacion}' style='color: #6366f1; word-break: break-all;'>{urlConfirmacion}</a>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #f9fafb; padding: 20px; border-top: 1px solid #e5e7eb;'>
                            <p style='margin: 0; color: #9ca3af; font-size: 13px;'>
                                © {DateTime.Now.Year} Task Management System.<br>
                                Este es un correo automático, por favor no respondas a este mensaje.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

                await _emailService.EnviarCorreoAsync(registro.Email, "Confirmación de Correo - Task Management", mensaje);

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Usuario registrado exitosamente. Por favor revise su correo para confirmar su cuenta.";
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
        [Route("ConfirmarCorreo")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarCorreo([FromQuery] string token, [FromQuery] string email)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.TokenConfirmacion == token);

                if (usuario == null)
                {
                    return BadRequest("Token o correo inválido.");
                }

                usuario.CorreoConfirmado = true;
                usuario.TokenConfirmacion = null;
                await _context.SaveChangesAsync();

                return Ok("Correo confirmado exitosamente. Ya puede iniciar sesión.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("OlvideContrasena")]
        [AllowAnonymous]
        public async Task<IActionResult> OlvideContrasena([FromBody] OlvideContrasenaDTO request)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (usuario == null)
                {
                    // No revelar si el correo existe o no por seguridad
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = true;
                    responseApi.Mensaje = "Si el correo está registrado, recibirá un enlace para restablecer su contraseña.";
                    return Ok(responseApi);
                }

                var tokenRecuperacion = Guid.NewGuid().ToString();
                usuario.TokenRecuperacion = tokenRecuperacion;
                usuario.FechaExpiracionToken = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

                await _context.SaveChangesAsync();

                // Enviar correo de recuperación
                // En un entorno real, la URL apuntaría a una página del cliente Blazor, no a la API directamente
                var urlRecuperacion = $"https://localhost:7041/restablecer-contrasena?token={tokenRecuperacion}&email={request.Email}";
                var mensaje = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Recuperación de Contraseña</title>
</head>
<body style='margin: 0; padding: 0; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f3f4f6; color: #333333;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f3f4f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); overflow: hidden; max-width: 600px; margin: 0 auto;'>
                    <tr>
                        <td align='center' style='background: linear-gradient(135deg, #6366f1, #8b5cf6); padding: 30px 20px;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: -0.5px;'>Task Management</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='margin-top: 0; color: #1f2937; font-size: 20px;'>Hola, {usuario.NombreCompleto}</h2>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.6; margin-bottom: 25px;'>
                                Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en Task Management. Si fuiste tú, haz clic en el botón de abajo para crear una nueva contraseña.
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center'>
                                        <a href='{urlRecuperacion}' style='display: inline-block; background: linear-gradient(135deg, #6366f1, #8b5cf6); color: #ffffff; text-decoration: none; padding: 14px 30px; border-radius: 8px; font-weight: 600; font-size: 16px; box-shadow: 0 4px 10px rgba(99,102,241,0.3);'>Restablecer Contraseña</a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #ef4444; font-size: 14px; line-height: 1.5; margin-top: 25px; text-align: center; font-weight: 500;'>
                                Este enlace expirará en 1 hora.
                            </p>
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Si no solicitaste este cambio, puedes ignorar este correo de forma segura. Tu contraseña actual seguirá siendo válida.
                            </p>
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:<br>
                                <a href='{urlRecuperacion}' style='color: #6366f1; word-break: break-all;'>{urlRecuperacion}</a>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='background-color: #f9fafb; padding: 20px; border-top: 1px solid #e5e7eb;'>
                            <p style='margin: 0; color: #9ca3af; font-size: 13px;'>
                                © {DateTime.Now.Year} Task Management System.<br>
                                Este es un correo automático, por favor no respondas a este mensaje.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

                await _emailService.EnviarCorreoAsync(request.Email, "Recuperación de Contraseña - Task Management", mensaje);

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Si el correo está registrado, recibirá un enlace para restablecer su contraseña.";
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        [HttpPost]
        [Route("RestablecerContrasena")]
        [AllowAnonymous]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDTO request)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email && u.TokenRecuperacion == request.Token);

                if (usuario == null || usuario.FechaExpiracionToken < DateTime.UtcNow)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Token inválido o expirado.";
                    return BadRequest(responseApi);
                }

                usuario.Clave = PasswordHelper.HashPassword(request.NuevaClave);
                usuario.TokenRecuperacion = null;
                usuario.FechaExpiracionToken = null;

                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Contraseña restablecida exitosamente.";
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
        [Route("Perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var responseApi = new ResponseAPI<PerfilUsuarioDTO>();

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return NotFound();
                }

                responseApi.EsCorrecto = true;
                responseApi.Valor = new PerfilUsuarioDTO
                {
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email ?? string.Empty
                };
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        [HttpPut]
        [Route("Perfil")]
        [Authorize]
        public async Task<IActionResult> ActualizarPerfil([FromBody] PerfilUsuarioDTO perfil)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Verificar si el nuevo email o nombre de usuario ya están en uso por otro usuario
                if (await _context.Usuarios.AnyAsync(u => u.Email == perfil.Email && u.UsuarioId != userId))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El correo electrónico ya está en uso por otro usuario.";
                    return BadRequest(responseApi);
                }

                if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == perfil.NombreUsuario && u.UsuarioId != userId))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El nombre de usuario ya está en uso por otro usuario.";
                    return BadRequest(responseApi);
                }

                usuario.NombreUsuario = perfil.NombreUsuario;
                usuario.NombreCompleto = perfil.NombreCompleto;

                // Si cambia el correo, podríamos requerir confirmación nuevamente, pero por simplicidad lo actualizamos
                usuario.Email = perfil.Email;
                usuario.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Perfil actualizado exitosamente.";
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        [HttpPost]
        [Route("CambiarContrasena")]
        [Authorize]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaPerfilDTO request)
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return NotFound();
                }

                if (!PasswordHelper.VerifyPassword(request.ClaveActual, usuario.Clave))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "La contraseña actual es incorrecta.";
                    return BadRequest(responseApi);
                }

                usuario.Clave = PasswordHelper.HashPassword(request.NuevaClave);
                usuario.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Contraseña cambiada exitosamente.";
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            var responseApi = new ResponseAPI<InicioSesionDTO>();

            try
            {
                var principal = ObtenerPrincipalDeTokenExpirado(request.Token);
                if (principal == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Token inválido.";
                    return BadRequest(responseApi);
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Token inválido.";
                    return BadRequest(responseApi);
                }

                var usuario = await _context.Usuarios.Include(u => u.IdRolNavigation).FirstOrDefaultAsync(u => u.UsuarioId == userId);

                if (usuario == null || usuario.RefreshToken != request.RefreshToken || usuario.RefreshTokenExpiracion <= DateTime.UtcNow)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Refresh token inválido o expirado.";
                    return BadRequest(responseApi);
                }

                var rolNombre = usuario.IdRolNavigation != null ? usuario.IdRolNavigation.NombreRol : "Sin Rol";

                // Generar nuevo Token JWT
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
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

                // Rotar Refresh Token
                var nuevoRefreshToken = GenerarRefreshToken();
                usuario.RefreshToken = nuevoRefreshToken;
                usuario.RefreshTokenExpiracion = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var inicioSesion = new InicioSesionDTO
                {
                    Nombre = usuario.NombreUsuario,
                    Correo = usuario.Email ?? string.Empty,
                    Rol = rolNombre,
                    RolId = usuario.RolId,
                    Token = tokenString,
                    RefreshToken = nuevoRefreshToken
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

        [HttpPost]
        [Route("RevocarToken")]
        [Authorize]
        public async Task<IActionResult> RevocarToken()
        {
            var responseApi = new ResponseAPI<bool>();

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.RefreshToken = null;
                usuario.RefreshTokenExpiracion = null;
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Token revocado exitosamente.";
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
        }

        private string GenerarRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? ObtenerPrincipalDeTokenExpirado(string token)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false, // No validar expiración aquí
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
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