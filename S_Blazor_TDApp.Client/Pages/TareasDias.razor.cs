using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class TareasDias : ComponentBase
    {
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public ITareaRecurrenteService TareaRecurrenteService { get; set; } = null!;
        [Inject] public ITareaDiasService TareaDiasService { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;

        [Parameter]
        public int IdTareasDiasAñadir { get; set; } = 0;

        private bool isSaving = false;
        private bool isLoading = true;

        private TareaDiasDTO tareaDias = new TareaDiasDTO();
        private TareasRecurrentesDTO tareaRecurrente = new TareasRecurrentesDTO();

        private List<TareasRecurrentesDTO> listaTareasRecurrentes = new();
        private List<TareaDiasDTO> listaTareasDias = new();

        private List<DiasDisponiblesDTO> listaDiasDisponibles = new()
        {
            new DiasDisponiblesDTO { DiaId = 1, NombreDia = "Lunes" },
            new DiasDisponiblesDTO { DiaId = 2, NombreDia = "Martes" },
            new DiasDisponiblesDTO { DiaId = 3, NombreDia = "Miércoles" },
            new DiasDisponiblesDTO { DiaId = 4, NombreDia = "Jueves" },
            new DiasDisponiblesDTO { DiaId = 5, NombreDia = "Viernes" },
            new DiasDisponiblesDTO { DiaId = 6, NombreDia = "Sábado" },
            new DiasDisponiblesDTO { DiaId = 7, NombreDia = "Domingo" }
        };

        private EditContext editContext = default!;

        private string tareaRecurrenteDisplay = string.Empty;
        private string horaDesdeString = string.Empty;
        private string horasHastaString = string.Empty;
        private string estadoString = string.Empty;
        private string tiempoEjecucionDisplay = string.Empty;
        private string cantidadEjecucionesDisplay = string.Empty;

        private int _selectedTareaId;
        private int selectedTareaId
        {
            get => _selectedTareaId;
            set
            {
                if (_selectedTareaId != value)
                {
                    _selectedTareaId = value;
                    tareaDias.TareaRecurrId = value;
                    ActualizarDatosTarea();
                    _ = CargarDiasPorTarea(value);
                }
            }
        }
        private int selectedDiaId { get; set; } = 0;

        private IEnumerable<DiasDisponiblesDTO> diasDisponibles =>
            listaDiasDisponibles.Where(d => listaTareasDias.All(td => td.Dia.DiaId != d.DiaId));

        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(tareaDias);
            try
            {
                isLoading = true;
                listaTareasRecurrentes = (await TareaRecurrenteService.Lista())
                                            .Where(t => t.Estado)
                                            .ToList();
                listaTareasDias = await TareaDiasService.ListaPorTareaRecurrId(tareaDias.TareaRecurrId);
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
                    Text = $"Error al cargar datos: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnValidSubmit()
        {
            isSaving = true;
            try
            {
                if (IdTareasDiasAñadir == 0)
                {
                    tareaDias.TareaRecurrId = _selectedTareaId;
                    if (selectedDiaId == 0)
                    {
                        await SwalService.FireAsync(new SweetAlertOptions
                        {
                            Title = "Error",
                            Text = "Debe seleccionar un día disponible.",
                            Icon = SweetAlertIcon.Error
                        });
                        return;
                    }
                    tareaDias.Dia = new DiasDisponiblesDTO { DiaId = selectedDiaId };

                    if (editContext.Validate())
                    {
                        await TareaDiasService.Guardar(tareaDias);
                        listaTareasDias = await TareaDiasService.ListaPorTareaRecurrId(_selectedTareaId);
                        selectedDiaId = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Ha ocurrido un error al guardar: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CargarDiasPorTarea(int tareaRecurrId)
        {
            listaTareasDias = await TareaDiasService.ListaPorTareaRecurrId(tareaRecurrId);
            StateHasChanged();
        }

        private void ActualizarDatosTarea()
        {
            if (selectedTareaId == 0)
            {
                tareaRecurrenteDisplay = horaDesdeString = horasHastaString = estadoString = tiempoEjecucionDisplay = cantidadEjecucionesDisplay = string.Empty;
            }
            else
            {
                tareaRecurrente = listaTareasRecurrentes.FirstOrDefault(t => t.TareaRecurrId == selectedTareaId) ?? new TareasRecurrentesDTO();
                tareaRecurrenteDisplay = tareaRecurrente.Recurrente ? "Si" : "No";
                horaDesdeString = tareaRecurrente.HoraDesde.ToString("hh:mm tt");
                horasHastaString = tareaRecurrente.HorasHasta.ToString("hh:mm tt");
                estadoString = tareaRecurrente.Estado ? "Activo" : "Inactivo";
                tiempoEjecucionDisplay = ConvertirTiempoEjecucion(tareaRecurrente.TiempoEjecucion);
                cantidadEjecucionesDisplay = $"{tareaRecurrente.CantidadEjecuciones} {(tareaRecurrente.CantidadEjecuciones == 1 ? "vez" : "veces")}";
            }
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

        private async Task Eliminar(int id, string nombreDia)
        {
            var result = await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar el día: {nombreDia}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    if (await TareaDiasService.Eliminar(id))
                    {
                        listaTareasDias.RemoveAll(td => td.TareaDiaId == id);
                    }
                }
                catch (Exception ex)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = $"Ha ocurrido un error al eliminar: {ex.Message}",
                        Icon = SweetAlertIcon.Error
                    });
                }
            }
        }
    }
}
