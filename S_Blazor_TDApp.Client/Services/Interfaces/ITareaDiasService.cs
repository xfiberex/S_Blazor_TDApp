using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface ITareaDiasService
    {
        Task<List<TareaDiasDTO>> Lista();
        Task<List<TareaDiasDTO>> ListaPorTareaRecurrId(int tareaRecurrId);
        Task<TareaDiasDTO> Buscar(int id);
        Task<int> Guardar(TareaDiasDTO empleado);
        Task<int> Editar(TareaDiasDTO empleado);
        Task<bool> Eliminar(int id);
    }
}