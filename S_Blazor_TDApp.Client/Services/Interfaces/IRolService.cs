using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IRolService
    {
        Task<List<RolDTO>> Lista();
        Task<RolDTO> Buscar(int id);
        Task<int> Guardar(RolDTO rol);
        Task<int> Editar(RolDTO rol);
        Task<bool> Eliminar(int id);
    }
}
