using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class PerfilUsuarioDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; } = null!;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = null!;
    }

    public class CambiarContrasenaPerfilDTO
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string ClaveActual { get; set; } = null!;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string NuevaClave { get; set; } = null!;

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [Compare(nameof(NuevaClave), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; } = null!;
    }
}
