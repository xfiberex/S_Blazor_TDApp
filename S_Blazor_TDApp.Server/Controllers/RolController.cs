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
    public class RolController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;

        public RolController(DbTdappContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseApi = new ResponseAPI<List<RolDTO>>();

            try
            {
                // Obtiene la lista de roles
                var listaRoles = await _context.Roles.ToListAsync();

                // Mapea la lista de entidades a una lista de DTOs
                var listaRolDTO = _mapper.Map<List<RolDTO>>(listaRoles);

                // Asignar la lista de roles al objeto de respuesta
                responseApi.EsCorrecto = true;
                responseApi.Valor = listaRolDTO;
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
            var responseApi = new ResponseAPI<RolDTO>();

            try
            {
                var rolEntity = await _context.Roles.FirstOrDefaultAsync(u => u.RolId == id);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                var rolDTO = _mapper.Map<RolDTO>(rolEntity);

                responseApi.EsCorrecto = true;
                responseApi.Valor = rolDTO;
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
        public async Task<IActionResult> Guardar(RolDTO rolDTO)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                // Mapea el DTO a la entidad utilizando AutoMapper
                var rolEntity = _mapper.Map<Rol>(rolDTO);

                // Asigna la fecha de creación y sin la de actualización al guardar
                rolEntity.FechaCreacion = DateTime.Now;
                rolEntity.FechaActualizacion = null;

                _context.Roles.Add(rolEntity);
                await _context.SaveChangesAsync();

                if (rolEntity.RolId != 0)
                {
                    responseApi.EsCorrecto = true;
                    responseApi.Valor = rolEntity.RolId;
                }
                else
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "No se pudo guardar el rol.";
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
        public async Task<IActionResult> Editar(RolDTO rolDTO, int id)
        {
            var responseApi = new ResponseAPI<int>();

            try
            {
                var rolEntity = await _context.Roles.FirstOrDefaultAsync(u => u.RolId == id);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                // Mapea los valores del DTO a la entidad existente
                _mapper.Map(rolDTO, rolEntity);

                // Asignamos la fecha de actualización solo al editar
                rolEntity.FechaActualizacion = DateTime.Now;

                _context.Roles.Update(rolEntity);
                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = rolEntity.RolId;
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
                var rolEntity = await _context.Roles.FirstOrDefaultAsync(u => u.RolId == id);

                if (rolEntity == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                _context.Roles.Remove(rolEntity);
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