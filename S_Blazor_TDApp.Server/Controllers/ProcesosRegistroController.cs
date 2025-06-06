﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesosRegistroController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;

        public ProcesosRegistroController(DbTdappContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region APIs para listar y buscar

        [HttpGet]
        [Route("ListaTareasRecurrentes")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareasRecurrentesDTO>>();

            try
            {
                var tareasRecurrentes = await _context.TareasRecurrentes.ToListAsync();

                var listaTareasRecurrentesDTO = _mapper.Map<List<TareasRecurrentesDTO>>(tareasRecurrentes);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaTareasRecurrentesDTO;
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
        [Route("ListaProcesos")]
        public async Task<IActionResult> ListaProcesos()
        {
            var responseApi = new ResponseAPI<List<RegistroProcesoDTO>>();

            try
            {
                var procesos = await _context.RegistroProcesos
                    .Include(rp => rp.RefTareaRecurr) // Asegúrate de incluir solo la relación con TareaRecurrente
                    .Include(rp => rp.RefUsuario) // Asegúrate de incluir solo la relación con Usuario
                    .ToListAsync();

                var listaProcesosDTO = _mapper.Map<List<RegistroProcesoDTO>>(procesos);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaProcesosDTO;
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
            var responseApi = new ResponseAPI<TareasRecurrentesDTO>();
            try
            {
                var tareaRecurrenteEntity = await _context.TareasRecurrentes
                                            .FirstOrDefaultAsync(tc => tc.TareaRecurrId == id);

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
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpGet]
        [Route("BuscarProcesos/{id}")]
        public async Task<IActionResult> BuscarProcesos(int id)
        {
            var responseApi = new ResponseAPI<RegistroProcesoDTO>();
            try
            {
                var procesosEntity = await _context.RegistroProcesos
                                            .FirstOrDefaultAsync(tc => tc.ProcesoId == id);

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
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        #endregion

        [HttpPost]
        [Route("GuardarProcesos")]
        public async Task<IActionResult> Guardar(RegistroProcesoDTO registroProcesosDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var procesoEntity = _mapper.Map<RegistroProceso>(registroProcesosDTO);

                _context.RegistroProcesos.Add(procesoEntity);
                await _context.SaveChangesAsync();

                if (procesoEntity.ProcesoId != 0)
                {
                    // Actualizar la tarea recurrente asociada para renovar su expiración.
                    var tarea = await _context.TareasRecurrentes
                                              .FirstOrDefaultAsync(t => t.TareaRecurrId == registroProcesosDTO.TareaRecurrId);

                    if (tarea != null && tarea.Recurrente)
                    {
                        // Se renueva la expiración: se actualiza la fecha de última renovación 
                        // y se marca el estado de expiración como activo (según la lógica de negocio, true = renovada).
                        tarea.FechaUltimaRenovacion = DateTime.Now;
                        tarea.EstadoExpiracion = true;
                        await _context.SaveChangesAsync();
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
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPost]
        [Route("RegistrarTareaCalendario")]
        public async Task<IActionResult> RegistrarTareaCalendario(TareasCalendarioCompletadoDTO calendarioDTO)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                // Buscar la tarea de calendario por su ID.
                var tarea = await _context.TareasCalendario.FirstOrDefaultAsync(t => t.TareaId == calendarioDTO.TareaId);
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
                    Fecha = DateTime.Now
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

                await _context.SaveChangesAsync();
                responseApi.EsCorrecto = true;
                responseApi.Valor = completado.TareaCompletoId;
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