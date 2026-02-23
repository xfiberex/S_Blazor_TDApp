using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net;
using System.Net.Http.Headers;

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
            // Intentar obtener el token actual
            var sesionUsuario = await _sessionStorage.ObtenerStorage<InicioSesionDTO>("sesionUsuario");
            
            if (sesionUsuario != null && !string.IsNullOrEmpty(sesionUsuario.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesionUsuario.Token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Si la respuesta es 401 (No autorizado) y no estamos ya intentando refrescar
            if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
            {
                _isRefreshing = true;

                try
                {
                    if (sesionUsuario != null && !string.IsNullOrEmpty(sesionUsuario.RefreshToken))
                    {
                        // Usar IServiceProvider para resolver IUsuarioService y evitar dependencias circulares
                        var usuarioService = _serviceProvider.GetRequiredService<IUsuarioService>();
                        
                        var nuevoTokenResponse = await usuarioService.RefreshToken(new RefreshTokenRequestDTO 
                        { 
                            Token = sesionUsuario.Token, 
                            RefreshToken = sesionUsuario.RefreshToken 
                        });

                        if (nuevoTokenResponse != null && !string.IsNullOrEmpty(nuevoTokenResponse.Token))
                        {
                            // Actualizar la sesión con los nuevos tokens
                            sesionUsuario.Token = nuevoTokenResponse.Token;
                            sesionUsuario.RefreshToken = nuevoTokenResponse.RefreshToken;
                            
                            await _sessionStorage.GuardarStorage("sesionUsuario", sesionUsuario);

                            // Actualizar el estado de autenticación
                            var authStateProvider = _serviceProvider.GetRequiredService<AuthenticationStateProvider>();
                            if (authStateProvider is AutenticacionExtension authExt)
                            {
                                await authExt.ActualizarEstadoAutenticacion(sesionUsuario);
                            }

                            // Reintentar la petición original con el nuevo token
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", nuevoTokenResponse.Token);
                            response = await base.SendAsync(request, cancellationToken);
                        }
                        else
                        {
                            // Si falla el refresh, cerrar sesión
                            await CerrarSesion();
                        }
                    }
                    else
                    {
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
