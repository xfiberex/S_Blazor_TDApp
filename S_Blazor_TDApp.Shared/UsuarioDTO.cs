using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class UsuarioDTO
    {
        public int UsuarioId { get; set; }

        public string Codigo { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 100 caracteres.")]
        public string NombreUsuario { get; set; } = null!;

        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string Clave { get; set; } = null!;

        [Required(ErrorMessage = "El correo electronico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string? Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el rol del usuario.")]
        public int RolId { get; set; }

        public string? NombreRol { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public string FechaActualizacionStr => FechaActualizacion.HasValue
        ? FechaActualizacion.Value.ToString("dd/MM/yyyy")
        : string.Empty;

        public RolDTO? Rol { get; set; }
    }
}