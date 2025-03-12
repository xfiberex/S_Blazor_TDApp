using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareaDiasController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;

        public TareaDiasController(DbTdappContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/TareaDias/Lista
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                // Incluir la navegación tanto a TareasRecurrente como a DiasDisponible
                var TareaDias = await _context.TareaDias
                                                  .Include(td => td.IdTareaRecurrNavegation)
                                                  .Include(td => td.IdDiaNavegation)
                                                  .ToListAsync();

                // Mapea la lista de entidades a una lista de DTOs
                var listaDiasDTO = _mapper.Map<List<TareaDiasDTO>>(TareaDias);

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaDiasDTO;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        // GET: api/TareaDias/ListaPorTarea?tareaRecurrId=...
        [HttpGet]
        [Route("ListaPorTarea")]
        public async Task<IActionResult> ListaPorTarea(int tareaRecurrId)
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                // Se filtra por TareaRecurrId y se incluye la navegación a DiasDisponible
                var listaDTO = await _context.TareaDias
                                             .Where(td => td.TareaRecurrId == tareaRecurrId)
                                             .Include(td => td.IdDiaNavegation)
                                             .Select(td => new TareaDiasDTO
                                             {
                                                 TareaDiaId = td.TareaDiaId,
                                                 TareaRecurrId = td.TareaRecurrId,
                                                 Dia = new DiasDisponiblesDTO
                                                 {
                                                     DiaId = td.IdDiaNavegation.DiaId,
                                                     NombreDia = td.IdDiaNavegation.NombreDia
                                                 }
                                             })
                                             .ToListAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaDTO;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        // GET: api/TareaDias/Buscar/{id}
        [HttpGet]
        [Route("Buscar/{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var responseApi = new ResponseAPI<TareaDiasDTO>();

            try
            {
                // Se incluye la navegación a DiasDisponible
                var entity = await _context.TareaDias
                                           .Include(td => td.IdTareaRecurrNavegation)
                                           .Include(td => td.IdDiaNavegation)
                                           .FirstOrDefaultAsync(td => td.TareaDiaId == id);

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
                        DiaId = entity.IdDiaNavegation.DiaId,
                        NombreDia = entity.IdDiaNavegation.NombreDia
                    }
                };

                responseApi.EsCorrecto = true;
                responseApi.Valor = dto;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        // POST: api/TareaDias/Guardar
        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(TareaDiasDTO tareaDiasDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Validar que exista la tarea recurrente
                var tareaRecurrente = await _context.TareasRecurrentes
                                                   .FirstOrDefaultAsync(tr => tr.TareaRecurrId == tareaDiasDTO.TareaRecurrId);

                if (tareaRecurrente == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "La tarea recurrente asociada no existe.";
                    return BadRequest(responseApi);
                }

                // Validar que exista el día en la tabla de días
                var diaDisponible = await _context.DiasDisponibles
                                                 .FirstOrDefaultAsync(d => d.DiaId == tareaDiasDTO.Dia.DiaId);

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
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = entity.TareaDiaId;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }

            return Ok(responseApi);
        }

        // PUT: api/TareaDias/Editar/{id}
        [HttpPut]
        [Route("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, TareaDiasDTO tareaDiasDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var entity = await _context.TareaDias
                                           .FirstOrDefaultAsync(td => td.TareaDiaId == id);
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
                                                        .FirstOrDefaultAsync(tr => tr.TareaRecurrId == tareaDiasDTO.TareaRecurrId);

                    if (tareaRecurrente == null)
                    {
                        responseApi.EsCorrecto = false;
                        responseApi.Mensaje = "La tarea recurrente asociada no existe.";
                        return BadRequest(responseApi);
                    }
                }

                // Validar que el día exista
                var diaDisponible = await _context.DiasDisponibles
                                                 .FirstOrDefaultAsync(d => d.DiaId == tareaDiasDTO.Dia.DiaId);

                if (diaDisponible == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El día asociado no existe en la tabla de días disponibles.";
                    return BadRequest(responseApi);
                }

                // Actualizar los campos
                entity.TareaRecurrId = tareaDiasDTO.TareaRecurrId;
                entity.DiaId = tareaDiasDTO.Dia.DiaId;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = entity.TareaDiaId;
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }

            return Ok(responseApi);
        }

        // DELETE: api/TareaDias/Eliminar/{id}
        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var entity = await _context.TareaDias.FirstOrDefaultAsync(td => td.TareaDiaId == id);
                if (entity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la entrada de Tarea Día.";
                    return NotFound(responseApi);
                }

                _context.TareaDias.Remove(entity);
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