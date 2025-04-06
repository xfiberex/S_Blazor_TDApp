using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface ITareaCalendarioService
    {
        Task<List<TareasCalendarioDTO>> Lista();
        Task<TareasCalendarioDTO> Buscar(int id);
        Task<int> Guardar(TareasCalendarioDTO tareaCalendario);
        Task<int> Editar(TareasCalendarioDTO tareaCalendario);
        Task<bool> Eliminar(int id);
    }
}