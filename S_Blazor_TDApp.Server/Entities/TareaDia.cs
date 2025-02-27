using S_Blazor_TDApp.Server.Entities;

public partial class TareaDia
{
    public int TareaDiaId { get; set; }

    public int TareaRecurrId { get; set; }

    // FK que apunta a la tabla de días
    public int DiaId { get; set; }

    // Propiedad de navegación a TareasRecurrente
    public virtual TareasRecurrente IdTareaRecurrNavegation { get; set; } = null!;

    // Propiedad de navegación a la tabla de días
    public virtual DiasDisponible IdDiaNavegation { get; set; } = null!;
}