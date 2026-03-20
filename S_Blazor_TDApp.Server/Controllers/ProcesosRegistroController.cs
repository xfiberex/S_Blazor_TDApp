using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/procesos-registro")]
    [ApiController]
    [Authorize(Roles = "Super_Administrador,Administrador,Supervisor,Empleado")]
    public class ProcesosRegistroController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcesosRegistroController> _logger;

        public ProcesosRegistroController(DbTdappContext context, IMapper mapper, ILogger<ProcesosRegistroController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        #region APIs para listar y buscar

        [HttpGet]
        public async Task<IActionResult> ListaProcesos([FromQuery] int pagina = 1, [FromQuery] int registrosPorPagina = 1000, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<PaginatedResultDTO<RegistroProcesoDTO>>();

            try
            {
                // Tareas Recurrentes
                var procesosRecurrentes = await _context.RegistroProcesos
                    .Include(rp => rp.RefTareaRecurr)
                    .Include(rp => rp.RefUsuario)
                    .AsNoTracking()
                    .ToListAsync(ct);

                var listaRecurrentesDTO = _mapper.Map<List<RegistroProcesoDTO>>(procesosRecurrentes);

                // Tareas Calendario Completadas
                var procesosCalendario = await _context.TareasCalendarioCompletados
                    .Include(tc => tc.RefTarea)
                    .Include(tc => tc.RefUsuario)
                    .AsNoTracking()
                    .ToListAsync(ct);

                var listaCalendarioDTO = procesosCalendario.Select(tc => new RegistroProcesoDTO
                {
                    ProcesoId = tc.TareaCompletoId,
                    TareaCalendarioId = tc.TareaId,
                    RefTareaCalendario = tc.RefTarea != null ? new TareasCalendarioDTO 
                    { 
                        TareaId = tc.RefTarea.TareaId, 
                        NombreTarea = tc.RefTarea.NombreTarea,
                        DescripcionTarea = tc.RefTarea.DescripcionTarea 
                    } : null,
                    UsuarioId = tc.UsuarioId ?? 0,
                    RefUsuario = tc.RefUsuario != null ? new UsuarioDTO 
                    { 
                        UsuarioId = tc.RefUsuario.UsuarioId, 
                        NombreUsuario = tc.RefUsuario.NombreUsuario 
                    } : null,
                    FechaRegistro = tc.Fecha,
                    DescripcionRegistro = tc.DescripcionTareaCompletado ?? string.Empty,
                    TipoTarea = "Calendario"
                }).ToList();

                var listaCompletaDTO = listaRecurrentesDTO.Concat(listaCalendarioDTO)
                                       .OrderByDescending(x => x.FechaRegistro)
                                       .ToList();

                var total = listaCompletaDTO.Count;
                var pagedList = listaCompletaDTO
                    .Skip((pagina - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToList();

                responseApi.EsCorrecto = true;
                responseApi.Valor = new PaginatedResultDTO<RegistroProcesoDTO>
                {
                    Items = pagedList,
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
        public async Task<IActionResult> BuscarProcesos(int id, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<RegistroProcesoDTO>();
            try
            {
                var procesosEntity = await _context.RegistroProcesos
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(tc => tc.ProcesoId == id, ct);

                if (procesosEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró el proceso.";
                    return NotFound(responseApi);
                }

                var registroProcesoDTO = _mapper.Map<RegistroProcesoDTO>(procesosEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = registroProcesoDTO;
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

        #endregion

        [HttpPost]
        public async Task<IActionResult> Guardar(RegistroProcesoDTO registroProcesosDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var procesoEntity = _mapper.Map<RegistroProceso>(registroProcesosDTO);

                _context.RegistroProcesos.Add(procesoEntity);
                await _context.SaveChangesAsync(ct);

                if (procesoEntity.ProcesoId != 0)
                {
                    // Actualizar la tarea recurrente asociada para renovar su expiración.
                    var tarea = await _context.TareasRecurrentes
                                              .FirstOrDefaultAsync(t => t.TareaRecurrId == registroProcesosDTO.TareaRecurrId, ct);

                    if (tarea != null && tarea.Recurrente)
                    {
                        // Se renueva la expiración: se actualiza la fecha de última renovación 
                        // y se marca el estado de expiración como activo (según la lógica de negocio, true = renovada).
                        tarea.FechaUltimaRenovacion = DateTime.UtcNow;
                        tarea.EstadoExpiracion = true;
                        await _context.SaveChangesAsync(ct);
                    }

                    responseApi.EsCorrecto = true;
                    responseApi.Valor = procesoEntity.ProcesoId;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se pudo guardar el proceso.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return CreatedAtAction(nameof(BuscarProcesos), new { id = responseApi.Valor }, responseApi);
        }

        [HttpPost("calendario-completado")]
        public async Task<IActionResult> RegistrarTareaCalendario(TareasCalendarioCompletadoDTO calendarioDTO, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                // Buscar la tarea de calendario por su ID.
                var tarea = await _context.TareasCalendario.FirstOrDefaultAsync(t => t.TareaId == calendarioDTO.TareaId, ct);
                if (tarea == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "Tarea de calendario no encontrada.";
                    return NotFound(responseApi);
                }

                // Registrar el completado en la tabla Tareas_Calendario_Completado.
                var completado = new TareasCalendarioCompletado
                {
                    TareaId = tarea.TareaId,
                    UsuarioId = calendarioDTO.UsuarioId,
                    EstadoCompletado = true, // Se marca como completado (según la lógica de negocio)
                    DescripcionTareaCompletado = calendarioDTO.DescripcionTareaCompletado,
                    Fecha = DateTime.UtcNow
                };
                _context.TareasCalendarioCompletados.Add(completado);

                // Si se suministran nuevos datos (nueva descripción, fecha y hora) se actualiza la tarea.
                if (calendarioDTO.RefTareaCalendario != null)
                {
                    // Validar que la nueva fecha y hora sean diferentes a las actuales
                    if (tarea.Fecha.Date == calendarioDTO.RefTareaCalendario.Fecha.Date &&
                        tarea.Hora.TimeOfDay == calendarioDTO.RefTareaCalendario.Hora.TimeOfDay)
                    {
                        responseApi.EsCorrecto = false;
                        responseApi.Mensaje = "La nueva fecha y hora no pueden ser iguales a las actuales.";
                        return BadRequest(responseApi);
                    }
                    tarea.DescripcionTarea = calendarioDTO.RefTareaCalendario.DescripcionTarea;
                    tarea.Fecha = calendarioDTO.RefTareaCalendario.Fecha;
                    tarea.Hora = calendarioDTO.RefTareaCalendario.Hora;
                }

                await _context.SaveChangesAsync(ct);
                responseApi.EsCorrecto = true;
                responseApi.Valor = completado.TareaCompletoId;
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