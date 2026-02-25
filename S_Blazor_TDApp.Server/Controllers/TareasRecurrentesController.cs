using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/tareas-recurrentes")]
    [ApiController]
    [Authorize(Roles = "Super_Administrador,Administrador,Supervisor,Empleado")]
    public class TareasRecurrentesController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TareasRecurrentesController> _logger;

        public TareasRecurrentesController(DbTdappContext context, IMapper mapper, ILogger<TareasRecurrentesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Lista([FromQuery] int pagina = 1, [FromQuery] int registrosPorPagina = 20, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<PaginatedResultDTO<TareasRecurrentesDTO>>();

            try
            {
                // Obtiene la lista de tareas recurrentes
                var query = _context.TareasRecurrentes.AsNoTracking();

                var total = await query.CountAsync(ct);

                var tareasRecurrentes = await query
                                            .Skip((pagina - 1) * registrosPorPagina)
                                            .Take(registrosPorPagina)
                                            .ToListAsync(ct);

                // Mapea la lista de entidades a una lista de DTOs
                var listaTareasRecurrentesDTO = _mapper.Map<List<TareasRecurrentesDTO>>(tareasRecurrentes);

                responseApi.EsCorrecto = true;
                responseApi.Valor = new PaginatedResultDTO<TareasRecurrentesDTO>
                {
                    Items = listaTareasRecurrentesDTO,
                    TotalRegistros = total,
                    Pagina = pagina,
                    RegistrosPorPagina = registrosPorPagina
                };
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
        public async Task<IActionResult> Buscar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<TareasRecurrentesDTO>();
            try
            {
                var tareaRecurrenteEntity = await _context.TareasRecurrentes
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(tc => tc.TareaRecurrId == id, ct);

                if (tareaRecurrenteEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                var tareaRecurrenteDTO = _mapper.Map<TareasRecurrentesDTO>(tareaRecurrenteEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaRecurrenteDTO;
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
        public async Task<IActionResult> Guardar(TareasRecurrentesDTO tareasRecurrentesDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var tareaRecurrenteEntity = _mapper.Map<TareasRecurrente>(tareasRecurrentesDTO);

                _context.TareasRecurrentes.Add(tareaRecurrenteEntity);
                await _context.SaveChangesAsync(ct);

                if (tareaRecurrenteEntity.TareaRecurrId != 0)
                {
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = tareaRecurrenteEntity.TareaRecurrId;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se pudo guardar la tarea.";
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
        public async Task<IActionResult> Editar(int id, TareasRecurrentesDTO tareasRecurrentesDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                var tareaRecurrenteEntity = await _context.TareasRecurrentes
                                            .FirstOrDefaultAsync(tc => tc.TareaRecurrId == id, ct);

                if (tareaRecurrenteEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(tareasRecurrentesDTO, tareaRecurrenteEntity);

                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaRecurrenteEntity.TareaRecurrId;
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
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                var tareaRecurrenteEntity = await _context.TareasRecurrentes
                                            .FirstOrDefaultAsync(tc => tc.TareaRecurrId == id, ct);

                if (tareaRecurrenteEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                _context.TareasRecurrentes.Remove(tareaRecurrenteEntity);
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