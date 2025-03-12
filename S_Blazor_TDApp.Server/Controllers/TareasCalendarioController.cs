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
    public class TareasCalendarioController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;

        public TareasCalendarioController(DbTdappContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<TareasCalendarioDTO>>();

            try
            {
                var tareasCalendario = await _context.TareasCalendarios.ToListAsync();

                // Mapea la lista de entidades a una lista de DTOs
                var listaTareasRecurrentesDTO = _mapper.Map<List<TareasCalendarioDTO>>(tareasCalendario);

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

                var tareaCalendarioDTO = _mapper.Map<TareasCalendarioDTO>(tareaCalendarioEntity);

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
        public async Task<IActionResult> Guardar(TareasCalendarioDTO tareasCalendarioDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var tareaCalendarioEntity = _mapper.Map<TareasCalendario>(tareasCalendarioDTO);

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
        public async Task<IActionResult> Editar(int id, TareasCalendarioDTO tareasCalendarioDTO)
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

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(tareasCalendarioDTO, tareaCalendarioEntity);

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