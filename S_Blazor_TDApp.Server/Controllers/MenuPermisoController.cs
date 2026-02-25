using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Server.Controllers
{
    [Route("api/menus")]
    [ApiController]
    [Authorize]
    public class MenuPermisoController : ControllerBase
    {
        private readonly DbTdappContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MenuPermisoController> _logger;

        // Nombre protegido: sus permisos no pueden modificarse
        private const string ROL_PROTEGIDO = "Super_Administrador";

        public MenuPermisoController(DbTdappContext context, IMapper mapper, ILogger<MenuPermisoController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Devuelve todos los menús disponibles en la aplicación.
        /// Accesible para cualquier usuario autenticado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TodosLosMenus(CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<List<MenuDTO>>();
            try
            {
                var menus = await _context.Menus
                    .AsNoTracking()
                    .OrderBy(m => m.Orden)
                    .ToListAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = _mapper.Map<List<MenuDTO>>(menus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }

        /// <summary>
        /// Devuelve los menús a los que tiene acceso el rol indicado.
        /// El Super_Administrador siempre recibe todos los menús, independientemente de la BD.
        /// </summary>
        [HttpGet("por-rol/{rolId:int}")]
        public async Task<IActionResult> MenusPorRol(int rolId, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<List<MenuDTO>>();
            try
            {
                var rol = await _context.Roles.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RolId == rolId, ct);

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
                        .ToListAsync(ct);
                }
                else
                {
                    menus = await _context.RolMenus
                        .AsNoTracking()
                        .Where(rm => rm.RolId == rolId)
                        .Include(rm => rm.Menu)
                        .Select(rm => rm.Menu)
                        .OrderBy(m => m.Orden)
                        .ToListAsync(ct);
                }

                responseApi.EsCorrecto = true;
                responseApi.Valor = _mapper.Map<List<MenuDTO>>(menus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }

        /// <summary>
        /// Actualiza los permisos de menú para un rol.
        /// Solo accesible para el rol Administrador.
        /// No se puede modificar al Super_Administrador.
        /// </summary>
        [HttpPut("permisos")]
        [Authorize(Roles = "Super_Administrador,Administrador")]
        public async Task<IActionResult> ActualizarPermisos([FromBody] ActualizarPermisosDTO dto, CancellationToken ct = default)
        {
            var responseApi = new ResponseAPI<bool>();
            try
            {
                var rol = await _context.Roles.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RolId == dto.RolId, ct);

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
                    .ToListAsync(ct);

                _context.RolMenus.RemoveRange(permisosActuales);

                // Insertar los nuevos permisos (sólo IDs de menús válidos)
                var menuIdsValidos = await _context.Menus
                    .Where(m => dto.MenuIds.Contains(m.MenuId))
                    .Select(m => m.MenuId)
                    .ToListAsync(ct);

                foreach (var menuId in menuIdsValidos)
                {
                    _context.RolMenus.Add(new RolMenu
                    {
                        RolId = dto.RolId,
                        MenuId = menuId
                    });
                }

                await _context.SaveChangesAsync(ct);

                responseApi.EsCorrecto = true;
                responseApi.Valor = true;
                responseApi.Mensaje = "Permisos actualizados correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                responseApi.EsCorrecto = false;
                responseApi.Mensaje = "Ocurrió un error interno. Intente nuevamente.";
                return StatusCode(500, responseApi);
            }
            return Ok(responseApi);
        }
    }
}
