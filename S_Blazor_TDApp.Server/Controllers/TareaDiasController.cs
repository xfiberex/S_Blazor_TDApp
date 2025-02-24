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

        public TareaDiasController(DbTdappContext context)
        {
            _context = context;
        }

        // GET: api/TareaDias/Lista
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                var listaEntities = await _context.TareaDias
                                                  .Include(td => td.TareaRecurr)
                                                  .ToListAsync();

                var listaDTO = listaEntities.Select(item => new TareaDiasDTO
                {
                    TareaDiaId = item.TareaDiaId,
                    TareaRecurrId = item.TareaRecurrId,
                    Dia = item.Dia
                }).ToList();

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

        // GET: api/TareaDias/ListaPorTarea
        [HttpGet]
        [Route("ListaPorTarea")]
        public async Task<IActionResult> ListaPorTarea(int tareaRecurrId)
        {
            var responseApi = new ResponseAPI<List<TareaDiasDTO>>();

            try
            {
                var listaDTO = await _context.TareaDias
                                             .Where(td => td.TareaRecurrId == tareaRecurrId)
                                             .Select(td => new TareaDiasDTO
                                             {
                                                 TareaDiaId = td.TareaDiaId,
                                                 TareaRecurrId = td.TareaRecurrId,
                                                 Dia = td.Dia
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
                var entity = await _context.TareaDias
                                           .Include(td => td.TareaRecurr)
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
                    Dia = entity.Dia
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
                var tareaRecurrente = await _context.TareasRecurrentes
                                                   .FirstOrDefaultAsync(tr => tr.TareaRecurrId == tareaDiasDTO.TareaRecurrId);
                if (tareaRecurrente == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "La tarea recurrente asociada no existe.";
                    return BadRequest(responseApi);
                }

                var entity = new TareaDia
                {
                    TareaRecurrId = tareaDiasDTO.TareaRecurrId,
                    Dia = tareaDiasDTO.Dia
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
                var entity = await _context.TareaDias.FirstOrDefaultAsync(td => td.TareaDiaId == id);
                if (entity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la entrada de Tarea Día.";
                    return NotFound(responseApi);
                }

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

                entity.TareaRecurrId = tareaDiasDTO.TareaRecurrId;
                entity.Dia = tareaDiasDTO.Dia;

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