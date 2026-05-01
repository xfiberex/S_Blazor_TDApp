using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class RefreshTokenRequestDTO
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
