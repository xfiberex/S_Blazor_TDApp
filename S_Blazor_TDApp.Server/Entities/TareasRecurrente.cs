namespace S_Blazor_TDApp.Server.Entities;

public partial class TareasRecurrente
{
    public int TareaRecurrId { get; set; }

    public string NombreTareaRecurr { get; set; } = null!;

    public string DescripcionTareaRecurr { get; set; } = null!;

    public bool Recurrente { get; set; }

    public DateTime HoraDesde { get; set; } = DateTime.UtcNow;

    public DateTime HorasHasta { get; set; } = DateTime.UtcNow;

    public int TiempoEjecucion { get; set; }

    public int CantidadEjecuciones { get; set; }

    public bool Estado { get; set; }
}
