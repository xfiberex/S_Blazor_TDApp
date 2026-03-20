using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Client.Services;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Pages.Views;

public partial class V_RolMenuPermisos : ComponentBase
{
    [Inject] private IRolService RolService { get; set; } = default!;
    [Inject] private IMenuPermisoService MenuPermisoService { get; set; } = default!;
    [Inject] private MenuPermisoEstado MenuEstado { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private SweetAlertService SwalService { get; set; } = default!;

    [Parameter]
    public int RolId { get; set; }

    private RolDTO? rolActual;
    private List<MenuDTO> todosLosMenus = new();
    private HashSet<int> menuIdsSeleccionados = new();

    private bool isLoading = true;
    private bool isSaving = false;
    private bool esRolProtegido = false;

    private const string ROL_PROTEGIDO = "Super_Administrador";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Inicia las tres tareas en paralelo antes de hacer await
            var tareaRol          = RolService.Buscar(RolId);
            var tareatodosMenus   = MenuPermisoService.TodosLosMenus();
            var tareaMenusActivos = MenuPermisoService.MenusPorRol(RolId);

            rolActual         = await tareaRol;
            todosLosMenus     = await tareatodosMenus;
            var menusActuales = await tareaMenusActivos;

            // Protege al Super_Administrador
            esRolProtegido = rolActual.NombreRol == ROL_PROTEGIDO;

            menuIdsSeleccionados = menusActuales.Select(m => m.MenuId).ToHashSet();
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
                Text = $"No se pudo cargar la información: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
            NavigationManager.NavigateTo("/rolesPermisos");
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnMenuToggle(int menuId, bool tieneAcceso)
    {
        if (tieneAcceso)
            menuIdsSeleccionados.Add(menuId);
        else
            menuIdsSeleccionados.Remove(menuId);
    }

    private async Task Guardar()
    {
        isSaving = true;
        try
        {
            var dto = new ActualizarPermisosDTO
            {
                RolId = RolId,
                MenuIds = menuIdsSeleccionados.ToList()
            };

            await MenuPermisoService.ActualizarPermisos(dto);

            // Invalida la caché para que el NavMenu refleje los cambios
            MenuEstado.Limpiar();

            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¡Guardado!",
                Text = $"Los permisos del rol '{rolActual?.NombreRol}' fueron actualizados correctamente.",
                Icon = SweetAlertIcon.Success
            });

            NavigationManager.NavigateTo("/rolesPermisos");
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"No se pudo actualizar los permisos: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            isSaving = false;
        }
    }
}
