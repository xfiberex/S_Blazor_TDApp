using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class Perfil : ComponentBase
    {
        [Inject] public IUsuarioService usuarioService { get; set; } = null!;

        private PerfilUsuarioDTO? usuario;
        private CambiarContrasenaPerfilDTO cambioClave = new();

        private bool isLoading = true;
        private bool isSavingPerfil = false;
        private bool isSavingClave = false;

        private string? mensajePerfil;
        private string? errorPerfil;
        private string? mensajeClave;
        private string? errorClave;

        protected override async Task OnInitializedAsync()
        {
            await CargarPerfil();
        }

        private async Task CargarPerfil()
        {
            try
            {
                isLoading = true;
                usuario = await usuarioService.ObtenerPerfil();
            }
            catch (Exception ex)
            {
                errorPerfil = "Error al cargar el perfil: " + ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ActualizarPerfil()
        {
            if (usuario == null) return;

            try
            {
                isSavingPerfil = true;
                errorPerfil = null;
                mensajePerfil = null;

                var actualizado = await usuarioService.ActualizarPerfil(usuario);
                if (actualizado)
                {
                    mensajePerfil = "Perfil actualizado correctamente.";
                }
            }
            catch (Exception ex)
            {
                errorPerfil = ex.Message;
            }
            finally
            {
                isSavingPerfil = false;
            }
        }

        private async Task CambiarContrasena()
        {
            try
            {
                isSavingClave = true;
                errorClave = null;
                mensajeClave = null;

                var exito = await usuarioService.CambiarContrasenaPerfil(cambioClave);
                if (exito)
                {
                    mensajeClave = "Contraseña actualizada correctamente.";
                    cambioClave = new CambiarContrasenaPerfilDTO();
                }
            }
            catch (Exception ex)
            {
                errorClave = ex.Message;
            }
            finally
            {
                isSavingClave = false;
            }
        }
    }
}
