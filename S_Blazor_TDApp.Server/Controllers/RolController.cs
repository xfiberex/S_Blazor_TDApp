using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = "Super_Administrador,Administrador,Supervisor")]
    public class RolController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RolController> _logger;

        public RolController(DbTdappContext context, IMapper mapper, ILogger<RolController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Super_Administrador,Administrador,Supervisor")]
        public async Task<IActionResult> Lista(CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<List<RolDTO>>();

            try
            {
                // Obtiene la lista de roles
                var listaRoles = await _context.Roles.AsNoTracking().ToListAsync(ct);

                // Mapea la lista de entidades a una lista de DTOs
                var listaRolDTO = _mapper.Map<List<RolDTO>>(listaRoles);

                // Asignar la lista de roles al objeto de respuesta
                responseApi.EsCorrecto = true;
                responseApi.Valor = listaRolDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Super_Administrador,Administrador,Supervisor")]
        public async Task<IActionResult> Buscar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<RolDTO>();

            try
            {
                var rolEntity = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(u => u.RolId == id, ct);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                var rolDTO = _mapper.Map<RolDTO>(rolEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = rolDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPost]
        [Authorize(Roles = "Super_Administrador,Administrador")]
        public async Task<IActionResult> Guardar(RolDTO rolDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var rolEntity = _mapper.Map<Rol>(rolDTO);

                // Asigna la fecha de creación y sin la de actualización al guardar
                rolEntity.FechaCreacion = DateTime.UtcNow;
                rolEntity.FechaActualizacion = null;

                _context.Roles.Add(rolEntity);
                await _context.SaveChangesAsync(ct);

                if (rolEntity.RolId != 0)
                {
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = rolEntity.RolId;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se pudo guardar el rol.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return CreatedAtAction(nameof(Buscar), new { id = responseApi.Valor }, responseApi);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Super_Administrador,Administrador")]
        public async Task<IActionResult> Editar(RolDTO rolDTO, int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var rolEntity = await _context.Roles.FirstOrDefaultAsync(u => u.RolId == id, ct);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                // Protección: no se puede renombrar el Super_Administrador
                if (rolEntity.NombreRol == "Super_Administrador")
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol Super_Administrador es protegido y no puede editarse.";
                    return BadRequest(responseApi);
                }

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(rolDTO, rolEntity);

                // Asignamos la fecha de actualización solo al editar
                rolEntity.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = rolEntity.RolId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Super_Administrador,Administrador")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var rolEntity = await _context.Roles.FirstOrDefaultAsync(u => u.RolId == id, ct);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                // Protección: no se puede eliminar el Super_Administrador
                if (rolEntity.NombreRol == "Super_Administrador")
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol Super_Administrador es protegido y no puede eliminarse.";
                    return BadRequest(responseApi);
                }

                _context.Roles.Remove(rolEntity);
                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }
    }
}