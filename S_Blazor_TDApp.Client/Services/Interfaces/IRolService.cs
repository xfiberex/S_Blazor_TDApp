using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IRolService
    {
        Task<List<RolDTO>> Lista();
    }
}
