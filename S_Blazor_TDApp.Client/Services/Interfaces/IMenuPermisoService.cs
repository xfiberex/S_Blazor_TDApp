using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IMenuPermisoService
    {
        /// <summary>Devuelve todos los menús disponibles en la aplicación.</summary>
        Task<List<MenuDTO>> TodosLosMenus();

        /// <summary>Devuelve los menús a los que tiene acceso el rol indicado.</summary>
        Task<List<MenuDTO>> MenusPorRol(int rolId);

        /// <summary>Actualiza los permisos de menú para un rol. Solo Administrador.</summary>
        Task<bool> ActualizarPermisos(ActualizarPermisosDTO dto);
    }
}
