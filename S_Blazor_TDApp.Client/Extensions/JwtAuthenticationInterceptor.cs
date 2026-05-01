using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net;

namespace S_Blazor_TDApp.Client.Extensions
{
    public class JwtAuthenticationInterceptor : DelegatingHandler
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly NavigationManager _navigationManager;
        private readonly IServiceProvider _serviceProvider;
        private bool _isRefreshing = false;

        public JwtAuthenticationInterceptor(
            ISessionStorageService sessionStorage,
            NavigationManager navigationManager,
            IServiceProvider serviceProvider)
        {
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            // Intentar obtener el token actual
            var sesionUsuario = await _sessionStorage.ObtenerStorage<InicioSesionDTO>("sesionUsuario");

            var response = await base.SendAsync(request, cancellationToken);

            // Si la respuesta es 401 (No autorizado) y no estamos ya intentando refrescar
            if (response.StatusCode != HttpStatusCode.Unauthorized ||
                _isRefreshing ||
                sesionUsuario == null)
            {
                return response;
            }

            _isRefreshing = true;

            try
            {
                // Usar IServiceProvider para resolver IUsuarioService y evitar dependencias circulares
                var usuarioService = _serviceProvider.GetRequiredService<IUsuarioService>();
                
                var nuevoTokenResponse = await usuarioService.RefreshToken();

                if (nuevoTokenResponse != null)
                {
                    await _sessionStorage.GuardarStorage("sesionUsuario", sesionUsuario);

                    // Actualizar el estado de autenticación
                    var authStateProvider = _serviceProvider.GetRequiredService<AuthenticationStateProvider>();
                    if (authStateProvider is AutenticacionExtension authExt)
                    {
                        await authExt.ActualizarEstadoAutenticacion(nuevoTokenResponse);
                    }

                    // Reintentar la petición original con la nueva cookie de acceso
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    // Si falla el refresh, cerrar sesión
                    await CerrarSesion();
                }
            }
            catch
            {
                await CerrarSesion();
            }
            finally
            {
                _isRefreshing = false;
            }

            return response;
        }

        private async Task CerrarSesion()
        {
            var authStateProvider = _serviceProvider.GetRequiredService<AuthenticationStateProvider>();
            if (authStateProvider is AutenticacionExtension authExt)
            {
                await authExt.ActualizarEstadoAutenticacion(null);
            }
            _navigationManager.NavigateTo("/", true);
        }
    }
}
