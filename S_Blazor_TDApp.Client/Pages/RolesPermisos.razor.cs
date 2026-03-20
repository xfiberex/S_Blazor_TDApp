using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class RolesPermisos : ComponentBase
    {
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public IRolService RolService { get; set; } = null!;
        [Inject] public Microsoft.JSInterop.IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;

        private List<RolDTO> listaRoles = new();
        private List<RolDTO> filteredRoles = new();
        private List<RolDTO> pagedRoles = new();

        private bool isLoading = true;
        private string searchTerm = "";

        private int currentPage = 1;
        private int pageSize = 10;
        private readonly int[] pageSizeOptions = { 10, 15, 20 };

        private System.Threading.Timer? _searchTimer;

        protected override async Task OnInitializedAsync()
        {
            await LoadRoles();
        }

        private async Task LoadRoles()
        {
            try
            {
                isLoading = true;
                listaRoles = await RolService.Lista();
                filteredRoles = listaRoles;
                currentPage = 1;
                UpdatePagedRoles();
            }
            catch (UnauthorizedAccessException)
            {
                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Error al cargar roles: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task Eliminar(int id, string nombreRol)
        {
            var result = await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar el rol {nombreRol}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    if (await RolService.Eliminar(id))
                    {
                        listaRoles.RemoveAll(u => u.RolId == id);
                        FilterRoles();
                        await SwalService.FireAsync(new SweetAlertOptions
                        {
                            Title = "Eliminado",
                            Text = "El rol ha sido eliminado correctamente.",
                            Icon = SweetAlertIcon.Success
                        });
                    }
                }
                catch (Exception ex)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = $"Ha ocurrido un error al eliminar el rol: {ex.Message}",
                        Icon = SweetAlertIcon.Error
                    });
                }
            }
        }

        private void OnSearchTermChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            _searchTimer?.Dispose();
            _searchTimer = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterRoles(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void FilterRoles()
        {
            filteredRoles = string.IsNullOrWhiteSpace(searchTerm)
                ? listaRoles
                : listaRoles.Where(u =>
                    u.NombreRol.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Descripcion?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

            currentPage = 1;
            UpdatePagedRoles();
        }

        private void ClearSearch()
        {
            searchTerm = string.Empty;
            FilterRoles();
        }

        private void UpdatePagedRoles()
        {
            pagedRoles = filteredRoles
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int newPageSize))
            {
                pageSize = newPageSize;
                currentPage = 1;
                UpdatePagedRoles();
            }
        }

        private void NextPage()
        {
            if (currentPage * pageSize < filteredRoles.Count)
            {
                currentPage++;
                UpdatePagedRoles();
            }
        }

        private void PrevPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagedRoles();
            }
        }
    }
}
