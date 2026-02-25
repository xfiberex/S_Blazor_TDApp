using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/tareas-calendario")]
    [ApiController]
    public class TareasCalendarioController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TareasCalendarioController> _logger;

        public TareasCalendarioController(DbTdappContext context, IMapper mapper, ILogger<TareasCalendarioController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Lista([FromQuery] int pagina = 1, [FromQuery] int registrosPorPagina = 20, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<PaginatedResultDTO<TareasCalendarioDTO>>();

            try
            {
                var query = _context.TareasCalendario
                                    .Include(tc => tc.TareasCalendarioCompletados)
                                    .AsNoTracking();

                var total = await query.CountAsync(ct);

                var tareasCalendario = await query
                                            .Skip((pagina - 1) * registrosPorPagina)
                                            .Take(registrosPorPagina)
                                            .ToListAsync(ct);

                // Mapea la lista de entidades a una lista de DTOs
                var listaTareasCalendarioDTO = _mapper.Map<List<TareasCalendarioDTO>>(tareasCalendario);

                responseApi.EsCorrecto = true;
                responseApi.Valor = new PaginatedResultDTO<TareasCalendarioDTO>
                {
                    Items = listaTareasCalendarioDTO,
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
            var responseApi = new ResponseAPI<TareasCalendarioDTO>();

            try
            {
                var tareaCalendarioEntity = await _context.TareasCalendario
                                                .Include(tc => tc.TareasCalendarioCompletados)
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id, ct);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                var tareaCalendarioDTO = _mapper.Map<TareasCalendarioDTO>(tareaCalendarioEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaCalendarioDTO;
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
        public async Task<IActionResult> Guardar(TareasCalendarioDTO tareasCalendarioDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var tareaCalendarioEntity = _mapper.Map<TareasCalendario>(tareasCalendarioDTO);

                _context.TareasCalendario.Add(tareaCalendarioEntity);
                await _context.SaveChangesAsync(ct);

                if (tareaCalendarioEntity.TareaId != 0)
                {
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = tareaCalendarioEntity.TareaId;
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
        public async Task<IActionResult> Editar(int id, TareasCalendarioDTO tareasCalendarioDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var tareaCalendarioEntity = await _context.TareasCalendario
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id, ct);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(tareasCalendarioDTO, tareaCalendarioEntity);

                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaCalendarioEntity.TareaId;
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
                var tareaCalendarioEntity = await _context.TareasCalendario
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id, ct);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                _context.TareasCalendario.Remove(tareaCalendarioEntity);
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