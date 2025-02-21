using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasRecurrentesController(DbTdappContext context) : ControllerBase
    {
        private readonly DbTdappContext _context = context;

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareasRecurrentesDTO>>();
            var listaTareasRecurrentesDTO = new List<TareasRecurrentesDTO>();

            try
            {
                var tareasRecurrentes = await _context.TareasRecurrentes.ToListAsync();

                foreach (var item in tareasRecurrentes)
                {
                    listaTareasRecurrentesDTO.Add(new TareasRecurrentesDTO
                    {
                        TareaRecurrId = item.TareaRecurrId,
                        NombreTareaRecurr = item.NombreTareaRecurr,
                        DescripcionTareaRecurr = item.DescripcionTareaRecurr,
                        Recurrente = item.Recurrente,
                        HoraDesde = item.HoraDesde,
                        HorasHasta = item.HorasHasta,
                        TiempoEjecucion = item.TiempoEjecucion,
                        CantidadEjecuciones = item.CantidadEjecuciones,
                        Estado = item.Estado
                    });
                }

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

                var tareaRecurrenteDTO = new TareasRecurrentesDTO
                {
                    TareaRecurrId = tareaRecurrenteEntity.TareaRecurrId,
                    NombreTareaRecurr = tareaRecurrenteEntity.NombreTareaRecurr,
                    DescripcionTareaRecurr = tareaRecurrenteEntity.DescripcionTareaRecurr,
                    Recurrente = tareaRecurrenteEntity.Recurrente,
                    HoraDesde = tareaRecurrenteEntity.HoraDesde,
                    HorasHasta = tareaRecurrenteEntity.HorasHasta,
                    TiempoEjecucion = tareaRecurrenteEntity.TiempoEjecucion,
                    CantidadEjecuciones = tareaRecurrenteEntity.CantidadEjecuciones,
                    Estado = tareaRecurrenteEntity.Estado
                };

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

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(TareasRecurrentesDTO tareasRecurrentes)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var tareaRecurrenteEntity = new TareasRecurrente
                {
                    NombreTareaRecurr = tareasRecurrentes.NombreTareaRecurr,
                    DescripcionTareaRecurr = tareasRecurrentes.DescripcionTareaRecurr,
                    Recurrente = tareasRecurrentes.Recurrente,
                    HoraDesde = tareasRecurrentes.HoraDesde,
                    HorasHasta = tareasRecurrentes.HorasHasta,
                    TiempoEjecucion = tareasRecurrentes.TiempoEjecucion,
                    CantidadEjecuciones = tareasRecurrentes.CantidadEjecuciones,
                    Estado = tareasRecurrentes.Estado
                };

                _context.TareasRecurrentes.Add(tareaRecurrenteEntity);
                await _context.SaveChangesAsync();

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
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPut]
        [Route("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, TareasRecurrentesDTO tareasRecurrentes)
        {
            var responseApi = new ResponseAPI<int>();

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

                tareasRecurrentes.TareaRecurrId = id;
                tareaRecurrenteEntity.NombreTareaRecurr = tareasRecurrentes.NombreTareaRecurr;
                tareaRecurrenteEntity.DescripcionTareaRecurr = tareasRecurrentes.DescripcionTareaRecurr;
                tareaRecurrenteEntity.Recurrente = tareasRecurrentes.Recurrente;
                tareaRecurrenteEntity.HoraDesde = tareasRecurrentes.HoraDesde;
                tareaRecurrenteEntity.HorasHasta = tareasRecurrentes.HorasHasta;
                tareaRecurrenteEntity.TiempoEjecucion = tareasRecurrentes.TiempoEjecucion;
                tareaRecurrenteEntity.CantidadEjecuciones = tareasRecurrentes.CantidadEjecuciones;
                tareaRecurrenteEntity.Estado = tareasRecurrentes.Estado;

                _context.Entry(tareaRecurrenteEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaRecurrenteEntity.TareaRecurrId;
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
                var tareaRecurrenteEntity = await _context.TareasRecurrentes
                                                .FirstOrDefaultAsync(tc => tc.TareaRecurrId == id);

                if (tareaRecurrenteEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                _context.TareasRecurrentes.Remove(tareaRecurrenteEntity);
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
