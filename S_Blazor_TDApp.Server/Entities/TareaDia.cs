using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class TareaDia
{
    public int TareaDiaId { get; set; }

    public int TareaRecurrId { get; set; }

    public int DiaId { get; set; }

    public virtual DiasDisponible IdDiaNavegation { get; set; } = null!;

    public virtual TareasRecurrente IdTareaRecurrNavegation { get; set; } = null!;
}
