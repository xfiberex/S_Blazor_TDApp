using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El correo electronico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Clave { get; set; } = string.Empty;
    }
}