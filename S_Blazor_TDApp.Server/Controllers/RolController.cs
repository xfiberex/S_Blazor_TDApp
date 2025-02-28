using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController(DbTdappContext context, IMapper mapper) : ControllerBase
    {
        private readonly DbTdappContext _context = context;
        private readonly IMapper _mapper = mapper;

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
    }
}