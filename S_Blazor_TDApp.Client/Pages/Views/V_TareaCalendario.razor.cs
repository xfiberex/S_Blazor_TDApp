using System.Globalization;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Client.Pages.Views;

public partial class V_TareaCalendario : ComponentBase
{
    [Inject] private ITareaCalendarioService TareaCalendarioService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private SweetAlertService SwalService { get; set; } = default!;

    [Parameter]
    public int IdTareaCalendarioEditar { get; set; } = 0;

    private EditContext editContext = null!;
    private string titulo = string.Empty;
    private string btnTexto = string.Empty;
    public bool isSaving = false;

    private TareasCalendarioDTO tareaCalendario = new TareasCalendarioDTO();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (IdTareaCalendarioEditar != 0)
            {
                tareaCalendario = await TareaCalendarioService.Buscar(IdTareaCalendarioEditar);
                btnTexto = "Actualizar";
                titulo = "Editar tarea de calendario";
            }
            else
            {
                btnTexto = "Guardar";
                titulo = "Nueva tarea de calendario";

                // Establece el estado predeterminado a 'Habilitado'
                tareaCalendario.Habilitado = true;
            }
            editContext = new EditContext(tareaCalendario);
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
            int idDevuelto = (IdTareaCalendarioEditar == 0)
                ? await TareaCalendarioService.Guardar(tareaCalendario)
                : await TareaCalendarioService.Editar(tareaCalendario);

            if (idDevuelto != 0)
            {
                NavigationManager.NavigateTo("/tareasCalendario");
            }
        }
        catch (Exception ex)
        {
            await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "Error",
                Text = $"Ha ocurrido un error al guardar la tarea de calendario: {ex.Message}",
                Icon = SweetAlertIcon.Error
            });
        }
        finally
        {
            isSaving = false;
        }
    }

    // Metodo para manejar el cambio de fecha
    private void OnDateChanged(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out DateTime selectedDate))
        {
            // Se asigna únicamente la fecha (hora = 00:00:00)
            tareaCalendario.Fecha = selectedDate.Date;
        }
    }

    private string horaInput
    {
        get => tareaCalendario.Hora.ToString("HH:mm:ss");
        set => _ = SetHoraInputAsync(value);
    }

    private async Task SetHoraInputAsync(string value)
    {
        try
        {
            if (DateTime.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                // Se conserva la fecha original de 'Hora' y se asigna solo el TimeOfDay
                tareaCalendario.Hora = tareaCalendario.Hora.Date.Add(parsedTime.TimeOfDay);
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
}
