using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/tarea-dias")]
    [ApiController]
    public class TareaDiasController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TareaDiasController> _logger;

        public TareaDiasController(DbTdappContext context, IMapper mapper, ILogger<TareaDiasController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/tarea-dias
        [HttpGet]
        public async Task<IActionResult> Lista(CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                // Incluir la navegación tanto a TareasRecurrente como a DiasDisponible
                var TareaDias = await _context.TareaDias
                                                  .Include(td => td.IdDiaNavigation)
                                                  .AsNoTracking()
                                                  .ToListAsync(ct);

                // Mapea la lista de entidades a una lista de DTOs
                var listaDiasDTO = _mapper.Map<List<TareaDiasDTO>>(TareaDias);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaDiasDTO;
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

        // GET: api/tarea-dias/por-tarea?tareaRecurrId=...
        [HttpGet("por-tarea")]
        public async Task<IActionResult> ListaPorTarea(int tareaRecurrId, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                // Se filtra por TareaRecurrId y se incluye la navegación a DiasDisponible
                var listaDTO = await _context.TareaDias
                                             .Where(td => td.TareaRecurrId == tareaRecurrId)
                                             .Include(td => td.IdDiaNavigation)
                                             .AsNoTracking()
                                             .Select(td => new TareaDiasDTO
                                             {
                                                 TareaDiaId = td.TareaDiaId,
                                                 TareaRecurrId = td.TareaRecurrId,
                                                 Dia = new DiasDisponiblesDTO
                                                 {
                                                     DiaId = td.IdDiaNavigation.DiaId,
                                                     NombreDia = td.IdDiaNavigation.NombreDia
                                                 }
                                             })
                                             .ToListAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaDTO;
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

        // GET: api/tarea-dias/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<TareaDiasDTO>();

            try
            {
                // Se incluye la navegación a DiasDisponible
                var entity = await _context.TareaDias
                                           .Include(td => td.IdDiaNavigation)
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(td => td.TareaDiaId == id, ct);

                if (entity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la entrada de Tarea Día.";
                    return NotFound(responseApi);
                }

                var dto = new TareaDiasDTO
                {
                    TareaDiaId = entity.TareaDiaId,
                    TareaRecurrId = entity.TareaRecurrId,
                    Dia = new DiasDisponiblesDTO
                    {
                        DiaId = entity.IdDiaNavigation.DiaId,
                        NombreDia = entity.IdDiaNavigation.NombreDia
                    }
                };

                responseApi.EsCorrecto = true;
                responseApi.Valor = dto;
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

        // POST: api/tarea-dias
        [HttpPost]
        public async Task<IActionResult> Guardar(TareaDiasDTO tareaDiasDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Validar que exista la tarea recurrente
                var tareaRecurrente = await _context.TareasRecurrentes
                                                   .FirstOrDefaultAsync(tr => tr.TareaRecurrId == tareaDiasDTO.TareaRecurrId, ct);

                if (tareaRecurrente == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "La tarea recurrente asociada no existe.";
                    return BadRequest(responseApi);
                }

                // Validar que exista el día en la tabla de días
                var diaDisponible = await _context.DiasDisponibles
                                                 .FirstOrDefaultAsync(d => d.DiaId == tareaDiasDTO.Dia.DiaId, ct);

                if (diaDisponible == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El día asociado no existe en la tabla de días disponibles.";
                    return BadRequest(responseApi);
                }

                // Crear la entidad con la FK
                var entity = new TareaDia
                {
                    TareaRecurrId = tareaDiasDTO.TareaRecurrId,
                    DiaId = tareaDiasDTO.Dia.DiaId
                };

                _context.TareaDias.Add(entity);
                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = entity.TareaDiaId;
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

        // PUT: api/tarea-dias/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, TareaDiasDTO tareaDiasDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var entity = await _context.TareaDias
                                           .FirstOrDefaultAsync(td => td.TareaDiaId == id, ct);
                if (entity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la entrada de Tarea Día.";
                    return NotFound(responseApi);
                }

                // Validar la tarea recurrente
                if (entity.TareaRecurrId != tareaDiasDTO.TareaRecurrId)
                {
                    var tareaRecurrente = await _context.TareasRecurrentes
                                                        .FirstOrDefaultAsync(tr => tr.TareaRecurrId == tareaDiasDTO.TareaRecurrId, ct);

                    if (tareaRecurrente == null)
                    {
                        responseApi.EsCorrecto = false;
                        responseApi.Mensaje = "La tarea recurrente asociada no existe.";
                        return BadRequest(responseApi);
                    }
                }

                // Validar que el día exista
                var diaDisponible = await _context.DiasDisponibles
                                                 .FirstOrDefaultAsync(d => d.DiaId == tareaDiasDTO.Dia.DiaId, ct);

                if (diaDisponible == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El día asociado no existe en la tabla de días disponibles.";
                    return BadRequest(responseApi);
                }

                // Actualizar los campos
                entity.TareaRecurrId = tareaDiasDTO.TareaRecurrId;
                entity.DiaId = tareaDiasDTO.Dia.DiaId;

                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = entity.TareaDiaId;
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

        // DELETE: api/tarea-dias/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var entity = await _context.TareaDias.FirstOrDefaultAsync(td => td.TareaDiaId == id, ct);
                if (entity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la entrada de Tarea Día.";
                    return NotFound(responseApi);
                }

                _context.TareaDias.Remove(entity);
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