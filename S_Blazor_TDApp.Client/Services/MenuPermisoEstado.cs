using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services
{
    /// <summary>
    /// Servicio de estado que almacena en caché los menús permitidos para el usuario activo.
    /// Se llena al iniciar el NavMenu y se invalida al cerrar sesión.
    /// </summary>
    public class MenuPermisoEstado
    {
        private readonly IMenuPermisoService _menuPermisoService;

        private List<MenuDTO>? _menusPermitidos;
        private int _rolIdCacheado = -1;

        public MenuPermisoEstado(IMenuPermisoService menuPermisoService)
        {
            _menuPermisoService = menuPermisoService;
        }

        /// <summary>
        /// Devuelve los menús permitidos para el rol.
        /// La primera vez hace la llamada a la API; las siguientes usa la caché.
        /// </summary>
        public async Task<List<MenuDTO>> ObtenerMenusPermitidos(int rolId)
        {
            if (_menusPermitidos != null && _rolIdCacheado == rolId)
                return _menusPermitidos;

            _menusPermitidos = await _menuPermisoService.MenusPorRol(rolId);
            _rolIdCacheado = rolId;
            return _menusPermitidos;
        }

        /// <summary>
        /// Indica si el usuario tiene permiso para ver el menú con la ruta indicada.
        /// </summary>
        public bool TieneAcceso(string ruta)
        {
            return _menusPermitidos?.Any(m =>
                string.Equals(m.Ruta, ruta, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Invalida la caché (p. ej. al cerrar sesión o cambiar de usuario).
        /// </summary>
        public void Limpiar()
        {
            _menusPermitidos = null;
            _rolIdCacheado = -1;
        }
    }
}
