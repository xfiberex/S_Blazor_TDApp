namespace S_Blazor_TDApp.Server.Entities;

public partial class TareasRecurrente
{
    public int TareaRecurrId { get; set; }

    public string NombreTareaRecurr { get; set; } = null!;

    public string DescripcionTareaRecurr { get; set; } = null!;

    public bool Recurrente { get; set; }

    public DateTime HoraDesde { get; set; }

    public DateTime HorasHasta { get; set; }

    public int TiempoEjecucion { get; set; }

    public int CantidadEjecuciones { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaUltimaRenovacion { get; set; } = DateTime.Now;

    public virtual ICollection<RegistroProceso> RegistroProcesos { get; set; } = new List<RegistroProceso>();

    public virtual ICollection<TareaDia> TareaDia { get; set; } = new List<TareaDia>();
}
