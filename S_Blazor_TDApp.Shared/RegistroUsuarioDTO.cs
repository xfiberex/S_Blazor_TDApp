using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class RegistroUsuarioDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; } = null!;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Clave { get; set; } = null!;
    }
}
