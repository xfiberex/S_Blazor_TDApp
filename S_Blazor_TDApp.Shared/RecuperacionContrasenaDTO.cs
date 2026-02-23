using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class OlvideContrasenaDTO
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = null!;
    }

    public class RestablecerContrasenaDTO
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string NuevaClave { get; set; } = null!;
    }
}
