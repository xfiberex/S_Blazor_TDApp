using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> Lista();
        Task<InicioSesionDTO> Login(LoginDTO login);
        Task<bool> Registro(RegistroUsuarioDTO registro);
        Task<string> ConfirmarCorreo(string token, string email);
        Task<bool> OlvideContrasena(OlvideContrasenaDTO request);
        Task<bool> RestablecerContrasena(RestablecerContrasenaDTO request);
        Task<PerfilUsuarioDTO> ObtenerPerfil();
        Task<bool> ActualizarPerfil(PerfilUsuarioDTO perfil);
        Task<bool> CambiarContrasenaPerfil(CambiarContrasenaPerfilDTO request);
        Task<InicioSesionDTO> RefreshToken();
        Task<bool> RevocarToken();
        Task<UsuarioDTO> Buscar(int id);
        Task<bool> ExisteCodigo(string codigo);
        Task<UsuarioDTO?> ObtenerPorEmail(string email);
        Task<int> Guardar(UsuarioDTO usuario);
        Task<int> Editar(UsuarioDTO usuario);
        Task<bool> Eliminar(int id);
        Task CambiarClave(int usuarioId, CambioClaveDTO cambioClaveDto);
    }
}