using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class TareasCalendario
{
    public int TareaId { get; set; }

    public string NombreTarea { get; set; } = null!;

    public string? DescripcionTarea { get; set; }

    public bool Habilitado { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public virtual ICollection<TareasCalendarioCompletado> TareasCalendarioCompletados { get; set; } = new List<TareasCalendarioCompletado>();
}
