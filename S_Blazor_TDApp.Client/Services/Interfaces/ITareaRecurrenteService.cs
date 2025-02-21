using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface ITareaRecurrenteService
    {
        Task<List<TareasRecurrentesDTO>> Lista();
        Task<TareasRecurrentesDTO> Buscar(int id);
        Task<int> Guardar(TareasRecurrentesDTO empleado);
        Task<int> Editar(TareasRecurrentesDTO empleado);
        Task<bool> Eliminar(int id);
    }
}