using Microsoft.AspNetCore.Components;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class TareasRecurrentes : ComponentBase
    {
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public ITareaRecurrenteService TareaRecurrenteService { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;

        private List<TareasRecurrentesDTO> listaTareasRecurrentes = new();
        private List<TareasRecurrentesDTO> filteredTasks = new();
        private List<TareasRecurrentesDTO> pagedTasks = new();

        private bool isLoading = true;
        private string searchTerm = "";
        private TareasRecurrentesDTO? expandedItem = null;

        private System.Threading.Timer? _searchTimerTasks;

        private int currentPage = 1;
        private int pageSize = 10;
        private readonly int[] pageSizeOptions = { 10, 15, 20 };

        protected override async Task OnInitializedAsync()
        {
            await LoadTareasRecurrentes();
        }

        private async Task LoadTareasRecurrentes()
        {
            try
            {
                isLoading = true;
                listaTareasRecurrentes = await TareaRecurrenteService.Lista();
                filteredTasks = listaTareasRecurrentes;
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
                    Text = $"Error al cargar las tareas recurrentes: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task Eliminar(int id, string nombreTareaRecurrente)
        {
            var result = await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar esta tarea recurrente: {nombreTareaRecurrente}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    if (await TareaRecurrenteService.Eliminar(id))
                    {
                        listaTareasRecurrentes.RemoveAll(u => u.TareaRecurrId == id);
                        FilterTasks();
                        await SwalService.FireAsync(new SweetAlertOptions
                        {
                            Title = "Eliminado",
                            Text = "La tarea recurrente ha sido eliminada correctamente.",
                            Icon = SweetAlertIcon.Success
                        });
                    }
                }
                catch (Exception ex)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = $"Ha ocurrido un error al eliminar la tarea recurrente: {ex.Message}",
                        Icon = SweetAlertIcon.Error
                    });
                }
            }
        }

        private void ToggleDescription(TareasRecurrentesDTO item)
        {
            if (expandedItem != null && expandedItem != item)
                expandedItem.ShowFullDescription = false;
            item.ShowFullDescription = !item.ShowFullDescription;
            expandedItem = item.ShowFullDescription ? item : null;
        }

        private void OnSearchTermChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            _searchTimerTasks?.Dispose();
            _searchTimerTasks = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterTasks(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void FilterTasks()
        {
            filteredTasks = string.IsNullOrWhiteSpace(searchTerm)
                ? listaTareasRecurrentes
                : listaTareasRecurrentes.Where(t =>
                    t.NombreTareaRecurr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.DescripcionTareaRecurr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                  ).ToList();
            currentPage = 1;
            UpdatePagedTasks();
        }

        private void ClearSearch()
        {
            searchTerm = string.Empty;
            FilterTasks();
        }

        private string ConvertirTiempoEjecucion(int minutos)
        {
            if (minutos >= 1440)
            {
                int dias = minutos / 1440;
                return $"{dias} {(dias > 1 ? "días" : "día")}";
            }
            else if (minutos >= 60)
            {
                int horas = minutos / 60;
                return $"{horas} {(horas > 1 ? "horas" : "hora")}";
            }
            else
            {
                return $"{minutos} min";
            }
        }

        private string ConvertirCantidadEjecuciones(int cantidad) =>
            cantidad == 1 ? $"{cantidad} vez" : $"{cantidad} veces";

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
