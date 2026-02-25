using System.Security.Claims;

namespace S_Blazor_TDApp.Server.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerarAccessToken(IEnumerable<Claim> claims);
        string GenerarRefreshToken();
        ClaimsPrincipal? ObtenerPrincipalDeTokenExpirado(string token);
    }
}
