using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuPermisoController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;

        // Nombre protegido: sus permisos no pueden modificarse
        private const string ROL_PROTEGIDO = "Super_Administrador";

        public MenuPermisoController(DbTdappContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Devuelve todos los menús disponibles en la aplicación.
        /// Accesible para cualquier usuario autenticado.
        /// </summary>
        [HttpGet]
        [Route("TodosLosMenus")]
        public async Task<IActionResult> TodosLosMenus()
        {
            var responseApi = new ResponseAPI<List<MenuDTO>>();
            try
            {
                var menus = await _context.Menus
                    .AsNoTracking()
                    .OrderBy(m => m.Orden)
                    .ToListAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = _mapper.Map<List<MenuDTO>>(menus);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        /// <summary>
        /// Devuelve los menús a los que tiene acceso el rol indicado.
        /// El Super_Administrador siempre recibe todos los menús, independientemente de la BD.
        /// </summary>
        [HttpGet]
        [Route("MenusPorRol/{rolId:int}")]
        public async Task<IActionResult> MenusPorRol(int rolId)
        {
            var responseApi = new ResponseAPI<List<MenuDTO>>();
            try
            {
                var rol = await _context.Roles.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RolId == rolId);

                if (rol == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                List<Menu> menus;

                // El Super_Administrador siempre tiene acceso a todos los menús
                if (rol.NombreRol == ROL_PROTEGIDO)
                {
                    menus = await _context.Menus
                        .AsNoTracking()
                        .OrderBy(m => m.Orden)
                        .ToListAsync();
                }
                else
                {
                    menus = await _context.RolMenus
                        .AsNoTracking()
                        .Where(rm => rm.RolId == rolId)
                        .Include(rm => rm.Menu)
                        .Select(rm => rm.Menu)
                        .OrderBy(m => m.Orden)
                        .ToListAsync();
                }

                responseApi.EsCorrecto = true;
                responseApi.Valor = _mapper.Map<List<MenuDTO>>(menus);
            }
            catch (Exception ex)
            {
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = ex.Message;
                return BadRequest(responseApi);
            }
            return Ok(responseApi);
        }

        /// <summary>
        /// Actualiza los permisos de menú para un rol.
        /// Solo accesible para el rol Administrador.
        /// No se puede modificar al Super_Administrador.
        /// </summary>
        [HttpPut]
        [Route("ActualizarPermisos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarPermisos([FromBody] ActualizarPermisosDTO dto)
        {
            var responseApi = new ResponseAPI<bool>();
            try
            {
                var rol = await _context.Roles.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RolId == dto.RolId);

                if (rol == null)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = "El rol no existe.";
                    return NotFound(responseApi);
                }

                // Protección: el Super_Administrador no puede modificarse
                if (rol.NombreRol == ROL_PROTEGIDO)
                {
                    responseApi.EsCorrecto = false;
                    responseApi.Mensaje = $"Los permisos del {ROL_PROTEGIDO} no pueden modificarse.";
                    return BadRequest(responseApi);
                }

                // Eliminar todos los permisos actuales del rol
                var permisosActuales = await _context.RolMenus
                    .Where(rm => rm.RolId == dto.RolId)
                    .ToListAsync();

                _context.RolMenus.RemoveRange(permisosActuales);

                // Insertar los nuevos permisos (sólo IDs de menús válidos)
                var menuIdsValidos = await _context.Menus
                    .Where(m => dto.MenuIds.Contains(m.MenuId))
                    .Select(m => m.MenuId)
                    .ToListAsync();

                foreach (var menuId in menuIdsValidos)
                {
                    _context.RolMenus.Add(new RolMenu
                    {
                        RolId = dto.RolId,
                        MenuId = menuId
                    });
                }

                await _context.SaveChangesAsync();

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Permisos actualizados correctamente.";
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
