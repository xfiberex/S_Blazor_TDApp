namespace S_Blazor_TDApp.Shared
{
    public class InicioSesionDTO
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}