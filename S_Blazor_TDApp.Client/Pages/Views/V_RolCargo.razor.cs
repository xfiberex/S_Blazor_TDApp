using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Pages.Views;

public partial class V_RolCargo : ComponentBase
{
    [Inject] private IRolService RolService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private SweetAlertService SwalService { get; set; } = default!;

    [Parameter]
    public int IdRolEditar { get; set; } = 0;

    private string titulo = string.Empty;
    private string btnTexto = string.Empty;
    public bool isSaving = false;

    private RolDTO rolCargo = new RolDTO();

    // Variables para mostrar el ID
    private string idRolDisplay = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (IdRolEditar != 0)
            {
                rolCargo = await RolService.Buscar(IdRolEditar);
                btnTexto = "Actualizar";
                titulo = "Editar rol";
                idRolDisplay = rolCargo.RolId.ToString();
            }
            else
            {
                btnTexto = "Guardar";
                titulo = "Nuevo rol";

                // Calcula el próximo ID basado en la lista de roles
                var roles = await RolService.Lista();
                int nextId = roles.Any() ? roles.Max(r => r.RolId) + 1 : 1;
                idRolDisplay = nextId.ToString();

                // Establece el estado predeterminado a 'Habilitado'
                rolCargo.Activo = true;
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

    private async Task OnValidSubmit()
    {
        isSaving = true;
        try
        {
            int idDevuelto = (IdRolEditar == 0)
                ? await RolService.Guardar(rolCargo)
                : await RolService.Editar(rolCargo);

            if (idDevuelto != 0)
            {
                NavigationManager.NavigateTo("/rolesPermisos");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Ha ocurrido un error al guardar el rol: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            isSaving = false;
        }
    }
}
