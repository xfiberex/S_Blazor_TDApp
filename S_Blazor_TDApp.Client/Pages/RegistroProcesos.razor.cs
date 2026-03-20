using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;
using System.Security.Claims;
using System.Globalization;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class RegistroProcesos : ComponentBase, IDisposable
    {
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public ITareaDiasService TareaDiasService { get; set; } = null!;
        [Inject] public ITareaRecurrenteService TareaRecurrenteService { get; set; } = null!;
        [Inject] public ITareaCalendarioService TareaCalendarioService { get; set; } = null!;
        [Inject] public IRegistroProcesosService RegistroProcesoService { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;
        [Inject] public AuthenticationStateProvider authStateProvider { get; set; } = null!;

        #region Variables y métodos de carga y actualización de datos

        // Listas de datos
        private List<RegistroProcesoDTO> listaRegistroProcesos = new List<RegistroProcesoDTO>();
        private List<TareasRecurrentesDTO> listaTareasRecurrentes = new List<TareasRecurrentesDTO>();
        private List<TareasCalendarioDTO> listaCalendario = new List<TareasCalendarioDTO>();

        // Listas filtradas para mostrar en la vista
        private List<TareasRecurrentesDTO> filteredTasks = new List<TareasRecurrentesDTO>();
        private List<RegistroProcesoDTO> filteredTasksProcess = new List<RegistroProcesoDTO>();
        private List<TareasCalendarioDTO> filteredTareaCalendario = new List<TareasCalendarioDTO>();

        // Datos para registrar procesos
        private RegistroProcesoDTO registroProceso = new RegistroProcesoDTO();
        private TareasCalendarioCompletadoDTO tareaCalendarioCompleto = new TareasCalendarioCompletadoDTO { RefTareaCalendario = new TareasCalendarioDTO() };
        private TareasCalendarioDTO tareaCalendario = new TareasCalendarioDTO { Fecha = DateTime.Now, Hora = DateTime.Now };

        // Variables de estado y filtros
        private bool isLoading = true;
        private bool isLoadingCalendario = true;

        private string searchTermTask = "";
        private string searchTermProcess = "";
        private string searchTermCalendar = "";
        private DateTime? startDate;

        private TareasRecurrentesDTO? expandedItem = null;
        private TareasCalendarioDTO? expandedItemCalendar = null;
        private TareasRecurrentesDTO? selectedTask = null;
        private TareasCalendarioDTO? selectedTareaCalendario = null;

        // Timers para debounce y refresco automático
        private System.Threading.Timer? _searchTimerTasks;
        private System.Threading.Timer? _searchTimerProcess;
        private System.Threading.Timer? _searchTimerCalendar;
        private System.Threading.Timer? _refreshTimer;

        // Diccionario para contar registros por tarea (por usuario actual)
        private Dictionary<int, int> userProcessCount = new();
        private int currentUserId;
        private EditContext editContext = null!;
        private EditContext editContextCalendario = null!;

        // Diccionario para almacenar los días asignados a cada tarea
        private Dictionary<int, List<TareaDiasDTO>> taskDiasMap = new();

        private async Task LoadTareaDiasForAllTasks()
        {
            taskDiasMap.Clear();
            foreach (var tarea in listaTareasRecurrentes)
            {
                var dias = await TareaDiasService.ListaPorTareaRecurrId(tarea.TareaRecurrId);
                taskDiasMap[tarea.TareaRecurrId] = dias;
            }
        }

        private string GetSpanishDayName(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => ""
        };

        private bool EstaProgramadaHoy(int tareaRecurrId)
        {
            if (taskDiasMap.ContainsKey(tareaRecurrId) && taskDiasMap[tareaRecurrId].Any())
            {
                var hoy = GetSpanishDayName(DateTime.Now.DayOfWeek);
                return taskDiasMap[tareaRecurrId].Any(d => d.Dia.NombreDia!.Equals(hoy, StringComparison.OrdinalIgnoreCase));
            }
            return true;
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedId))
                {
                    currentUserId = parsedId;
                }
            }

            await LoadTareasRecurrentes();
            await LoadTareasCalendario();
            await LoadProcesosRegistrados();
            await LoadTareaDiasForAllTasks();

            _refreshTimer = new System.Threading.Timer(async _ =>
            {
                await InvokeAsync(async () =>
                {
                    await RefreshData();
                    StateHasChanged();
                });
            }, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private async Task RefreshData()
        {
            try
            {
                listaTareasRecurrentes = (await TareaRecurrenteService.Lista())
                                            .Where(t => t.Estado)
                                            .ToList();
                filteredTasks = listaTareasRecurrentes;

                listaRegistroProcesos = (await RegistroProcesoService.ListaProcesos())
                                            .OrderByDescending(p => p.FechaRegistro)
                                            .ToList();
                filteredTasksProcess = listaRegistroProcesos;

                BuildUserProcessCount();
                await LoadTareaDiasForAllTasks();
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error al refrescar datos",
                    Text = ex.Message,
                    Icon = SweetAlertIcon.Error
                });
            }
        }

        #endregion


        #region Métodos para cargar tareas, procesos y registrar procesos

        private async Task LoadTareasRecurrentes()
        {
            try
            {
                isLoading = true;
                listaTareasRecurrentes = (await TareaRecurrenteService.Lista())
                                            .Where(t => t.Estado)
                                            .ToList();
                await LoadUserTaskOrder();
                FilterTasks();
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

        private async Task LoadTareasCalendario()
        {
            try
            {
                isLoadingCalendario = true;
                listaCalendario = (await TareaCalendarioService.Lista())
                                            .Where(t => t.Habilitado)
                                            .ToList();
                filteredTareaCalendario = listaCalendario;
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Error al cargar tareas de calendario: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoadingCalendario = false;
                StateHasChanged();
            }
        }

        private async Task LoadProcesosRegistrados()
        {
            try
            {
                isLoading = true;
                listaRegistroProcesos = (await RegistroProcesoService.ListaProcesos())
                                            .OrderByDescending(p => p.FechaRegistro)
                                            .ToList();
                filteredTasksProcess = listaRegistroProcesos;
                BuildUserProcessCount();
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
                    Text = $"Error al cargar los procesos registrados: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void BuildUserProcessCount()
        {
            userProcessCount = listaRegistroProcesos
                .Where(rp => rp.TipoTarea != "Calendario")
                .GroupBy(rp => rp.TareaRecurrId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count(rp => rp.UsuarioId == currentUserId)
                );
        }

        private int GetUserProcessCount(int tareaRecurrId) =>
            userProcessCount.ContainsKey(tareaRecurrId) ? userProcessCount[tareaRecurrId] : 0;

        private async Task OpenModal(TareasRecurrentesDTO task)
        {
            selectedTask = task;
            registroProceso = new RegistroProcesoDTO
            {
                TareaRecurrId = task.TareaRecurrId,
                UsuarioId = currentUserId,
                FechaRegistro = DateTime.Now
            };
            await JSRuntime.InvokeVoidAsync("showModal", "registroProcesoModal");
        }

        private async Task RegistrarProceso()
        {
            editContext = new EditContext(registroProceso);
            try
            {
                registroProceso.FechaRegistro = DateTime.Now;
                await RegistroProcesoService.GuardarProcesos(registroProceso);
                await JSRuntime.InvokeVoidAsync("hideModalAsync", "registroProcesoModal");
                await LoadProcesosRegistrados();
                await LoadTareasRecurrentes();
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Error al registrar el proceso: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
        }

        public void Dispose()
        {
            _searchTimerTasks?.Dispose();
            _searchTimerProcess?.Dispose();
            _searchTimerCalendar?.Dispose();
            _refreshTimer?.Dispose();
        }

        #endregion


        #region Métodos relacionados con las tareas de calendario

        private bool IsTaskAvailable(TareasCalendarioDTO task)
        {
            var scheduled = new DateTime(task.Fecha.Year, task.Fecha.Month, task.Fecha.Day, task.Hora.Hour, task.Hora.Minute, task.Hora.Second);
            return DateTime.Now >= scheduled && task.Habilitado;
        }

        private async Task OpenValidarTareaCalendarioModal(TareasCalendarioDTO task)
        {
            selectedTareaCalendario = task;
            tareaCalendarioCompleto = new TareasCalendarioCompletadoDTO
            {
                TareaId = task.TareaId,
                UsuarioId = currentUserId,
                RefTareaCalendario = new TareasCalendarioDTO
                {
                    TareaId = task.TareaId,
                    NombreTarea = task.NombreTarea,
                    DescripcionTarea = task.DescripcionTarea,
                    Fecha = task.Fecha,
                    Hora = task.Hora,
                    Habilitado = task.Habilitado
                }
            };
            await JSRuntime.InvokeVoidAsync("showModal", "validarTareaCalendarioModal");
        }

        private async Task ValidarTareaCalendario()
        {
            editContextCalendario = new EditContext(tareaCalendarioCompleto);

            if (selectedTareaCalendario != null)
            {
                if (selectedTareaCalendario.Fecha.Date == tareaCalendarioCompleto.RefTareaCalendario!.Fecha.Date &&
                   selectedTareaCalendario.Hora.TimeOfDay == tareaCalendarioCompleto.RefTareaCalendario.Hora.TimeOfDay)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Validación",
                        Text = "La nueva fecha y hora no pueden ser iguales a las actuales.",
                        Icon = SweetAlertIcon.Warning
                    });
                    return;
                }
            }
            try
            {
                await RegistroProcesoService.RegistrarTareaCalendario(tareaCalendarioCompleto);
                await JSRuntime.InvokeVoidAsync("hideModalAsync", "validarTareaCalendarioModal");
                await LoadTareasCalendario();
                await LoadProcesosRegistrados();
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Éxito",
                    Text = "La tarea de calendario ha sido validada.",
                    Icon = SweetAlertIcon.Success
                });
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Error al validar la tarea de calendario: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
        }

        private void OnDateChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out DateTime selectedDate))
            {
                tareaCalendarioCompleto.RefTareaCalendario!.Fecha = selectedDate.Date;
            }
        }

        private string horaInput
        {
            get => tareaCalendarioCompleto.RefTareaCalendario!.Hora.ToString("HH:mm:ss");
            set => _ = SetHoraInputAsync(value);
        }

        private async Task SetHoraInputAsync(string value)
        {
            try
            {
                if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
                {
                    tareaCalendarioCompleto.RefTareaCalendario!.Hora = tareaCalendarioCompleto.RefTareaCalendario.Hora.Date.Add(parsedTime.TimeOfDay);
                }
                else
                {
                    throw new FormatException("El formato de la hora debe ser 'HH:mm:ss'.");
                }
            }
            catch (FormatException ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error de formato",
                    Text = ex.Message,
                    Icon = SweetAlertIcon.Error
                });
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Ha ocurrido un error al guardar la hora: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
        }

        #endregion


        #region Métodos de filtrado, conversión de tiempos y descripción

        private void ToggleDescription(TareasRecurrentesDTO item)
        {
            if (expandedItem != null && expandedItem != item)
                expandedItem.ShowFullDescription = false;
            item.ShowFullDescription = !item.ShowFullDescription;
            expandedItem = item.ShowFullDescription ? item : null;
        }

        private void ToggleDescriptionCalendar(TareasCalendarioDTO itemCalendar)
        {
            if (expandedItemCalendar != null && expandedItemCalendar != itemCalendar)
                expandedItemCalendar.ShowFullDescription = false;
            itemCalendar.ShowFullDescription = !itemCalendar.ShowFullDescription;
            expandedItemCalendar = itemCalendar.ShowFullDescription ? itemCalendar : null;
        }

        private void OnSearchTermTaskChanged(ChangeEventArgs e)
        {
            searchTermTask = e.Value?.ToString() ?? string.Empty;
            _searchTimerTasks?.Dispose();
            _searchTimerTasks = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterTasks(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void OnSearchTermTaskProcessChanged(ChangeEventArgs e)
        {
            searchTermProcess = e.Value?.ToString() ?? string.Empty;
            _searchTimerProcess?.Dispose();
            _searchTimerProcess = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterTasksProcess(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void OnSearchTermTaskCalendarChanged(ChangeEventArgs e)
        {
            searchTermCalendar = e.Value?.ToString() ?? string.Empty;
            _searchTimerCalendar?.Dispose();
            _searchTimerCalendar = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() => { FilterTasksCalendar(); StateHasChanged(); });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void FilterTasks()
        {
            filteredTasks = listaTareasRecurrentes.Where(t =>
                string.IsNullOrWhiteSpace(searchTermTask) ||
                t.NombreTareaRecurr.Contains(searchTermTask, StringComparison.OrdinalIgnoreCase) ||
                t.DescripcionTareaRecurr.Contains(searchTermTask, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        private void FilterTasksProcess()
        {
            filteredTasksProcess = listaRegistroProcesos.Where(t =>
                (
                    string.IsNullOrWhiteSpace(searchTermProcess) ||
                    (t.TipoTarea == "Calendario" && (t.RefTareaCalendario?.NombreTarea?.Contains(searchTermProcess, StringComparison.OrdinalIgnoreCase) ?? false)) ||
                    (t.TipoTarea != "Calendario" && (t.RefTareaRecurr?.NombreTareaRecurr?.Contains(searchTermProcess, StringComparison.OrdinalIgnoreCase) ?? false)) ||
                    (t.DescripcionRegistro?.Contains(searchTermProcess, StringComparison.OrdinalIgnoreCase) ?? false)
                )
                &&
                (!startDate.HasValue || t.FechaRegistro.Date == startDate.Value.Date)
            ).ToList();
        }

        private void FilterTasksCalendar()
        {
            filteredTareaCalendario = listaCalendario.Where(t =>
                string.IsNullOrWhiteSpace(searchTermCalendar) ||
                t.NombreTarea.Contains(searchTermCalendar, StringComparison.OrdinalIgnoreCase) ||
                t.DescripcionTarea!.Contains(searchTermCalendar, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        private void ClearSearch()
        {
            searchTermTask = string.Empty;
            FilterTasks();
        }

        private void ClearSearchProcess()
        {
            searchTermProcess = string.Empty;
            startDate = null;
            FilterTasksProcess();
        }

        private void ClearSearchCalendar()
        {
            searchTermCalendar = string.Empty;
            FilterTasksCalendar();
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

        #region Drag and Drop para orden persistente

        private string OrderStorageKey => $"user_task_order_{currentUserId}";
        private TareasRecurrentesDTO? _draggedTask;

        private async Task LoadUserTaskOrder()
        {
            try
            {
                var savedOrderJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", OrderStorageKey);
                if (!string.IsNullOrEmpty(savedOrderJson))
                {
                    var savedOrder = System.Text.Json.JsonSerializer.Deserialize<List<int>>(savedOrderJson);
                    if (savedOrder != null && savedOrder.Any())
                    {
                        var orderedList = new List<TareasRecurrentesDTO>();
                        foreach (var id in savedOrder)
                        {
                            var task = listaTareasRecurrentes.FirstOrDefault(t => t.TareaRecurrId == id);
                            if (task != null) orderedList.Add(task);
                        }
                        var missingTasks = listaTareasRecurrentes.Where(t => !savedOrder.Contains(t.TareaRecurrId));
                        orderedList.AddRange(missingTasks);
                        listaTareasRecurrentes = orderedList;
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar errores de parseo de localStorage
            }
        }

        private async Task SaveUserTaskOrder()
        {
            var currentOrder = listaTareasRecurrentes.Select(t => t.TareaRecurrId).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(currentOrder);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", OrderStorageKey, json);
        }

        private void HandleDragStart(TareasRecurrentesDTO task)
        {
            _draggedTask = task;
        }

        private async Task HandleDrop(TareasRecurrentesDTO targetTask)
        {
            if (_draggedTask == null || _draggedTask.TareaRecurrId == targetTask.TareaRecurrId)
            {
                _draggedTask = null;
                return;
            }

            var originalIndex = listaTareasRecurrentes.IndexOf(_draggedTask);
            var targetIndex = listaTareasRecurrentes.IndexOf(targetTask);

            if (originalIndex >= 0 && targetIndex >= 0)
            {
                listaTareasRecurrentes.RemoveAt(originalIndex);
                var insertIndex = listaTareasRecurrentes.IndexOf(targetTask);
                if (originalIndex < targetIndex)
                    insertIndex++;
                listaTareasRecurrentes.Insert(insertIndex, _draggedTask);
                FilterTasks();
                await SaveUserTaskOrder();
            }

            _draggedTask = null;
        }

        #endregion

        #endregion
    }
}
