namespace S_Blazor_TDApp.Server.Entities;

public partial class TareasCalendario
{
    public int TareaId { get; set; }

    public string NombreTarea { get; set; } = null!;

    public string? DescripcionTarea { get; set; }

    public bool Habilitado { get; set; }

    public DateTime Fecha { get; set; }

    public DateTime Hora { get; set; } = DateTime.UtcNow;
}
