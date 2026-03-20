using System.Globalization;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Pages.Views;

public partial class V_TareaRecurrente : ComponentBase
{
    [Inject] private ITareaRecurrenteService TareaRecurrenteService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private SweetAlertService SwalService { get; set; } = default!;

    [Parameter]
    public int IdTareaRecurrenteEditar { get; set; } = 0;

    private EditContext editContext = null!;
    private string titulo = string.Empty;
    private string btnTexto = string.Empty;
    public bool isSaving = false;

    private TareasRecurrentesDTO tareaRecurrente = new TareasRecurrentesDTO();

    private bool recurrente
    {
        get => tareaRecurrente.Recurrente;
        set
        {
            tareaRecurrente.Recurrente = value;
            // Si es recurrente, se fija la cantidad de ejecuciones en 1 por defecto.
            tareaRecurrente.CantidadEjecuciones = value ? 1 : tareaRecurrente.CantidadEjecuciones;
            CalcularTiempoEjecucion();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (IdTareaRecurrenteEditar != 0)
            {
                tareaRecurrente = await TareaRecurrenteService.Buscar(IdTareaRecurrenteEditar);
                btnTexto = "Actualizar";
                titulo = "Editar tarea recurrente";
            }
            else
            {
                btnTexto = "Guardar";
                titulo = "Nueva tarea recurrente";

                // Establece el estado predeterminado a 'Habilitado'
                tareaRecurrente.Estado = true;
            }
            editContext = new EditContext(tareaRecurrente);
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
            int idDevuelto = (IdTareaRecurrenteEditar == 0)
                ? await TareaRecurrenteService.Guardar(tareaRecurrente)
                : await TareaRecurrenteService.Editar(tareaRecurrente);

            if (idDevuelto != 0)
            {
                NavigationManager.NavigateTo("/tareasRecurrentes");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Ha ocurrido un error al guardar la tarea recurrente: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            isSaving = false;
        }
    }

    private string horaDesdeInput
    {
        get => tareaRecurrente.HoraDesde.ToString("HH:mm:ss");
        set => _ = SetHoraDesdeInputAsync(value);
    }

    private string horaHastaInput
    {
        get => tareaRecurrente.HorasHasta.ToString("HH:mm:ss");
        set => _ = SetHoraHastaInputAsync(value);
    }

    private async Task SetHoraDesdeInputAsync(string value)
    {
        try
        {
            if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                tareaRecurrente.HoraDesde = tareaRecurrente.HoraDesde.Date.Add(parsedTime.TimeOfDay);
                CalcularTiempoEjecucion();
            }
            else
            {
                throw new FormatException("El formato de la hora debe ser 'HH:mm:ss'.");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = ex.Message,
                Icon = SweetAlertIcon.Error
            });
        }
    }

    private async Task SetHoraHastaInputAsync(string value)
    {
        try
        {
            if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                tareaRecurrente.HorasHasta = tareaRecurrente.HorasHasta.Date.Add(parsedTime.TimeOfDay);
                CalcularTiempoEjecucion();
            }
            else
            {
                throw new FormatException("El formato de la hora debe ser 'HH:mm:ss'.");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = ex.Message,
                Icon = SweetAlertIcon.Error
            });
        }
    }

    private void CalcularTiempoEjecucion()
    {
        if (tareaRecurrente.HoraDesde != default && tareaRecurrente.HorasHasta != default)
        {
            var diffMinutes = (tareaRecurrente.HorasHasta - tareaRecurrente.HoraDesde).TotalMinutes;
            tareaRecurrente.TiempoEjecucion = (int)diffMinutes;
        }
    }
}
