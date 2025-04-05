using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class RegistroProceso
{
    public int ProcesoId { get; set; }

    public int TareaRecurrId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string DescripcionRegistro { get; set; } = null!;

    public virtual TareasRecurrente RefTareaRecurr { get; set; } = null!;

    public virtual Usuario RefUsuario { get; set; } = null!;
}
