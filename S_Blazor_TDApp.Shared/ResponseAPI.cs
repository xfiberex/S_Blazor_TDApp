using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class ResponseAPI<T>
    {
        public bool EsCorrecto { get; set; }
        public T? Valor { get; set; }
        public string? Mensaje { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioDTO Usuario { get; set; } = null!;
    }
}