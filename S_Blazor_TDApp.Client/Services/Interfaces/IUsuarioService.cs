using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> Lista();
        Task<UsuarioDTO> Buscar(int id);
        Task<bool> ExisteCodigo(string codigo);
        Task<UsuarioDTO?> ObtenerPorEmail(string email);
        Task<int> Guardar(UsuarioDTO empleado);
        Task<int> Editar(UsuarioDTO empleado);
        Task<bool> Eliminar(int id);
        Task CambiarClave(int usuarioId, CambioClaveDTO cambioClaveDto);
    }
}