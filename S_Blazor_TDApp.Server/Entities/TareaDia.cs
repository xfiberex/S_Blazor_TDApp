namespace S_Blazor_TDApp.Server.Entities;

public partial class TareaDia
{
    public int TareaDiaId { get; set; }

    public int TareaRecurrId { get; set; }

    public string Dia { get; set; } = null!;

    public virtual TareasRecurrente TareaRecurr { get; set; } = null!;
}
