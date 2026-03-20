using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Pages.Views;

public partial class V_Usuario : ComponentBase
{
    [Inject] private IUsuarioService UsuarioService { get; set; } = default!;
    [Inject] private IRolService RolService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private SweetAlertService SwalService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider authStateProvider { get; set; } = default!;

    [Parameter]
    public int IdUsuarioEditar { get; set; } = 0;

    private string titulo = string.Empty;
    private string btnTexto = string.Empty;
    public bool isSaving = false;

    private UsuarioDTO usuario = new UsuarioDTO();
    private List<RolDTO> listaRoles = new List<RolDTO>();

    // Variables para mostrar el ID y Código del usuario
    private string idUsuarioDisplay = string.Empty;
    private string codigoUsuarioDisplay = string.Empty;

    // Variables para el modal de cambio de contraseña
    private bool mostrarModalCambioClave = false;
    private CambioClaveDTO cambioClaveModel = new CambioClaveDTO();

    // Variables para almacenar el ID del usuario autenticado y los valores originales de Email y Rol
    private int currentUserId;
    private string currentRol = string.Empty;
    private string originalEmail = string.Empty;
    private int originalRolId;

    // Roles que el Supervisor no puede asignar
    private static readonly HashSet<string> RolesRestringidosParaSupervisor =
        new() { "Super_Administrador", "Administrador" };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(idClaim))
                    currentUserId = int.Parse(idClaim);

                currentRol = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
            }

            var todosLosRoles = await RolService.Lista();

            // El Supervisor solo puede asignar roles de igual o menor jerarquía
            listaRoles = currentRol == "Supervisor"
                ? todosLosRoles.Where(r => !RolesRestringidosParaSupervisor.Contains(r.NombreRol)).ToList()
                : todosLosRoles;

            if (IdUsuarioEditar != 0)
            {
                usuario = await UsuarioService.Buscar(IdUsuarioEditar);
                btnTexto = "Actualizar";
                titulo = "Editar Usuario";
                idUsuarioDisplay = usuario.UsuarioId.ToString();
                codigoUsuarioDisplay = usuario.Codigo;

                originalEmail = usuario.Email!;
                originalRolId = usuario.RolId;
            }
            else
            {
                btnTexto = "Guardar";
                titulo = "Nuevo Usuario";

                var usuarios = await UsuarioService.Lista();
                int nextId = usuarios.Any() ? usuarios.Max(u => u.UsuarioId) + 1 : 1;
                idUsuarioDisplay = nextId.ToString();

                await GenerarCodigoUsuario();

                usuario.Activo = true;
            }
        }
        catch (UnauthorizedAccessException)
        {
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Error al cargar datos: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
    }

    private async Task GenerarCodigoUsuario()
    {
        string codigo;
        bool existe;
        var random = new Random();
        do
        {
            codigo = random.Next(10000, 100000).ToString();
            existe = await UsuarioService.ExisteCodigo(codigo);
        }
        while (existe);

        codigoUsuarioDisplay = codigo;
        usuario.Codigo = codigo;
    }

    private async Task OnValidSubmit()
    {
        isSaving = true;
        try
        {
            int idDevuelto = (IdUsuarioEditar == 0)
                ? await UsuarioService.Guardar(usuario)
                : await UsuarioService.Editar(usuario);

            if (idDevuelto != 0)
            {
                NavigationManager.NavigateTo("/usuarios");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Error al guardar el usuario: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            isSaving = false;
        }
    }

    private void AbrirModalCambioClave()
    {
        cambioClaveModel = new CambioClaveDTO();
        mostrarModalCambioClave = true;
    }

    private void CerrarModalCambioClave()
    {
        mostrarModalCambioClave = false;
    }

    private async Task GuardarCambioClave()
    {
        try
        {
            await UsuarioService.CambiarClave(usuario.UsuarioId, new CambioClaveDTO
            {
                UsuarioId = usuario.UsuarioId,
                NuevaClave = cambioClaveModel.NuevaClave,
                ConfirmarClave = cambioClaveModel.ConfirmarClave
            });

            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Éxito",
                Text = "La contraseña se actualizó correctamente.",
                Icon = SweetAlertIcon.Success
            });
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Error al cambiar la contraseña: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            mostrarModalCambioClave = false;
        }
    }
}
