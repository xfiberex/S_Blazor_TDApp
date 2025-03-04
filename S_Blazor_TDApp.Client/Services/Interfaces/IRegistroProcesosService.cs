using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IRegistroProcesosService
    {
        Task<List<TareasRecurrentesDTO>> Lista();
        Task<List<RegistroProcesoDTO>> ListaProcesos();
        Task<TareasRecurrentesDTO> Buscar(int id);
        Task<RegistroProcesoDTO> BuscarProcesos(int id);
        Task<int> GuardarProcesos(RegistroProcesoDTO proceso);
    }
}
