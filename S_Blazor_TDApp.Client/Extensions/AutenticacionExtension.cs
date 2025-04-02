using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using S_Blazor_TDApp.Shared;
using System.Security.Claims;

namespace S_Blazor_TDApp.Client.Extensions
{
    public class AutenticacionExtension : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _sinInformacion = new ClaimsPrincipal(new ClaimsIdentity());

        public AutenticacionExtension(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        /// <summary>
        /// Actualiza el estado de autenticación del usuario.
        /// Si el usuario es válido, guarda la información del usuario en el almacenamiento de sesión.
        /// Si el usuario es nulo, elimina la información del usuario del almacenamiento de sesión.
        /// </summary>
        /// <param name="sesionDTO">El objeto UsuarioDTO que contiene la información del usuario.</param>
        public async Task ActualizarEstadoAutenticacion(InicioSesionDTO? sesionDTO)
        {
            ClaimsPrincipal claimsPrincipal;

            if (sesionDTO != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name,sesionDTO.Nombre),
                    new Claim(ClaimTypes.Email,sesionDTO.Correo!),
                    new Claim(ClaimTypes.Role,sesionDTO.Rol)
                }, "JwtAuth"));

                await _sessionStorage.GuardarStorage("sesionUsuario", sesionDTO);
            }
            else
            {
                claimsPrincipal = _sinInformacion;
                await _sessionStorage.RemoveItemAsync("sesionUsuario");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        /// <summary>
        /// Obtiene el estado de autenticación actual del usuario.
        /// Si hay información de usuario en el almacenamiento de sesión, crea un ClaimsPrincipal con los datos del usuario.
        /// Si no hay información de usuario, devuelve un ClaimsPrincipal sin información.
        /// </summary>
        /// <returns>El estado de autenticación actual del usuario.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var sesionUsuario = await _sessionStorage.ObtenerStorage<InicioSesionDTO>("sesionUsuario");

            if (sesionUsuario == null)
                return await Task.FromResult(new AuthenticationState(_sinInformacion));

            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name,sesionUsuario.Nombre),
                    new Claim(ClaimTypes.Email,sesionUsuario.Correo!),
                    new Claim(ClaimTypes.Role,sesionUsuario.Rol)
                }, "JwtAuth"));

            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }
    }
}