using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class DiasDisponible
{
    public int DiaId { get; set; }

    public string NombreDia { get; set; } = null!;

    public virtual ICollection<TareaDia> TareaDia { get; set; } = new List<TareaDia>();
}
