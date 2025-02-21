using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly DbTdappContext _context;

        public RolController(DbTdappContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var responseAPI = new ResponseAPI<List<RolDTO>>();
            var listaRolDTO = new List<RolDTO>();

            try
            {
                // Recuperar la lista de roles
                foreach (var item in await _context.Roles.ToListAsync())
                {
                    listaRolDTO.Add(new RolDTO
                    {
                        RolId = item.RolId,
                        NombreRol = item.NombreRol,
                        Descripcion = item.Descripcion,
                        Activo = item.Activo,
                        FechaCreacion = item.FechaCreacion,
                        FechaActualizacion = item.FechaActualizacion
                    });
                }

                // Asignar la lista de roles al objeto de respuesta
                responseAPI.EsCorrecto = true;
                responseAPI.Valor = listaRolDTO;
            }
            catch (Exception ex)
            {
                responseAPI.EsCorrecto = false;
                responseAPI.Mensaje = ex.Message;
                return BadRequest(responseAPI);
            }
            return Ok(responseAPI);
        }
    }
}