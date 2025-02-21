using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface ITareaCalendarioService
    {
        Task<List<TareasCalendarioDTO>> Lista();
        Task<TareasCalendarioDTO> Buscar(int id);
        Task<int> Guardar(TareasCalendarioDTO empleado);
        Task<int> Editar(TareasCalendarioDTO empleado);
        Task<bool> Eliminar(int id);
    }
}