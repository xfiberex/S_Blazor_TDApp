using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesosRegistroController(DbTdappContext context, IMapper mapper) : ControllerBase
    {
        private readonly DbTdappContext _context = context;
        private readonly IMapper _mapper = mapper;

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
                var procesos = await _context.RegistroProcesos.ToListAsync();
                
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

    }
}