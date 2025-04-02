using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class CambioClaveDTO
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string NuevaClave { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarClave { get; set; } = string.Empty;
    }
}