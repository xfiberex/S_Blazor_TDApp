using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasCalendarioController(DbTdappContext context) : ControllerBase
    {
        private readonly DbTdappContext _context = context;

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareasCalendarioDTO>>();
            var listaTareasCalendarioDTO = new List<TareasCalendarioDTO>();

            try
            {
                var tareasCalendario = await _context.TareasCalendarios.ToListAsync();

                foreach (var item in tareasCalendario)
                {
                    listaTareasCalendarioDTO.Add(new TareasCalendarioDTO
                    {
                        TareaId = item.TareaId,
                        NombreTarea = item.NombreTarea,
                        DescripcionTarea = item.DescripcionTarea,
                        Habilitado = item.Habilitado,
                        Fecha = item.Fecha,
                        Hora = item.Hora
                    });
                }

                responseApi.EsCorrecto = true;
                responseApi.Valor = listaTareasCalendarioDTO;
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
            var responseApi = new ResponseAPI<TareasCalendarioDTO>();

            try
            {
                var tareaCalendarioEntity = await _context.TareasCalendarios
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                var tareaCalendarioDTO = new TareasCalendarioDTO
                {
                    TareaId = tareaCalendarioEntity.TareaId,
                    NombreTarea = tareaCalendarioEntity.NombreTarea,
                    DescripcionTarea = tareaCalendarioEntity.DescripcionTarea,
                    Habilitado = tareaCalendarioEntity.Habilitado,
                    Fecha = tareaCalendarioEntity.Fecha,
                    Hora = tareaCalendarioEntity.Hora
                };

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaCalendarioDTO;
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
        public async Task<IActionResult> Guardar(TareasCalendarioDTO tareasCalendario)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var tareaCalendarioEntity = new TareasCalendario
                {
                    NombreTarea = tareasCalendario.NombreTarea,
                    DescripcionTarea = tareasCalendario.DescripcionTarea,
                    Habilitado = tareasCalendario.Habilitado,
                    Fecha = tareasCalendario.Fecha,
                    Hora = tareasCalendario.Hora
                };

                _context.TareasCalendarios.Add(tareaCalendarioEntity);
                await _context.SaveChangesAsync();

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
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        [HttpPut]
        [Route("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, TareasCalendarioDTO tareasCalendario)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var tareaCalendarioEntity = await _context.TareasCalendarios
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                tareaCalendarioEntity.NombreTarea = tareasCalendario.NombreTarea;
                tareaCalendarioEntity.DescripcionTarea = tareasCalendario.DescripcionTarea;
                tareaCalendarioEntity.Habilitado = tareasCalendario.Habilitado;
                tareaCalendarioEntity.Fecha = tareasCalendario.Fecha;
                tareaCalendarioEntity.Hora = tareasCalendario.Hora;

                _context.Entry(tareaCalendarioEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = tareaCalendarioEntity.TareaId;
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
                var tareaCalendarioEntity = await _context.TareasCalendarios
                                                .FirstOrDefaultAsync(tc => tc.TareaId == id);

                if (tareaCalendarioEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se encontró la tarea.";
                    return NotFound(responseApi);
                }

                _context.TareasCalendarios.Remove(tareaCalendarioEntity);
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