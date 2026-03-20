using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class TareasCalendario : ComponentBase
    {
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public ITareaCalendarioService TareaCalendarioService { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;

        private List<TareasCalendarioDTO> listaTareasCalendario = new();
        private List<TareasCalendarioDTO> filteredTasks = new();
        private List<TareasCalendarioDTO> pagedTasks = new();

        private bool isLoading = true;
        private string searchTerm = "";
        private TareasCalendarioDTO? expandedItem = null;

        private System.Threading.Timer? _searchTimer;

        private int currentPage = 1;
        private int pageSize = 10;
        private readonly int[] pageSizeOptions = { 10, 15, 20 };

        protected override async Task OnInitializedAsync()
        {
            await LoadTareasCalendario();
        }

        private async Task LoadTareasCalendario()
        {
            try
            {
                isLoading = true;
                listaTareasCalendario = await TareaCalendarioService.Lista();
                filteredTasks = listaTareasCalendario;
                UpdatePagedTasks();
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
                    Text = $"Error al cargar las tareas de calendario: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task Eliminar(int id, string nombreTareaCalendario)
        {
            var result = await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar la tarea de calendario: {nombreTareaCalendario}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    if (await TareaCalendarioService.Eliminar(id))
                    {
                        listaTareasCalendario.RemoveAll(u => u.TareaId == id);
                        FilterTasks();
                        await SwalService.FireAsync(new SweetAlertOptions
                        {
                            Title = "Eliminado",
                            Text = "La tarea de calendario ha sido eliminada correctamente.",
                            Icon = SweetAlertIcon.Success
                        });
                    }
                }
                catch (Exception ex)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = $"Ha ocurrido un error al eliminar la tarea de calendario: {ex.Message}",
                        Icon = SweetAlertIcon.Error
                    });
                }
            }
        }

        private void ToggleDescription(TareasCalendarioDTO item)
        {
            if (expandedItem != null && expandedItem != item)
                expandedItem.ShowFullDescription = false;
            item.ShowFullDescription = !item.ShowFullDescription;
            expandedItem = item.ShowFullDescription ? item : null;
        }

        private void OnSearchTermChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            _searchTimer?.Dispose();
            _searchTimer = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterTasks(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void FilterTasks()
        {
            filteredTasks = string.IsNullOrWhiteSpace(searchTerm)
                ? listaTareasCalendario
                : listaTareasCalendario.Where(t =>
                    t.NombreTarea.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.DescripcionTarea?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            currentPage = 1;
            UpdatePagedTasks();
        }

        private void ClearSearch()
        {
            searchTerm = string.Empty;
            FilterTasks();
        }

        private void UpdatePagedTasks()
        {
            pagedTasks = filteredTasks
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
                UpdatePagedTasks();
            }
        }

        private void NextPage()
        {
            if (currentPage * pageSize < filteredTasks.Count)
            {
                currentPage++;
                UpdatePagedTasks();
            }
        }

        private void PrevPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagedTasks();
            }
        }
    }
}
