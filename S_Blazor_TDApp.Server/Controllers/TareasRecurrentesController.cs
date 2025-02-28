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
    public class TareasRecurrentesController(DbTdappContext context, IMapper mapper) : ControllerBase
    {
        private readonly DbTdappContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareasRecurrentesDTO>>();

            try
            {
                // Obtiene la lista de tareas recurrentes
                var tareasRecurrentes = await _context.TareasRecurrentes.ToListAsync();

                // Mapea la lista de entidades a una lista de DTOs
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

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar(TareasRecurrentesDTO tareasRecurrentesDTO)
        {
            var responseApi = new ResponseAPI<int>();
            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var tareaRecurrenteEntity = _mapper.Map<TareasRecurrente>(tareasRecurrentesDTO);

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
        public async Task<IActionResult> Editar(int id, TareasRecurrentesDTO tareasRecurrentesDTO)
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

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(tareasRecurrentesDTO, tareaRecurrenteEntity);

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