namespace S_Blazor_TDApp.Shared
{
    public class RolDTO
    {
        public int RolId { get; set; }

        public string NombreRol { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }
    }
}
