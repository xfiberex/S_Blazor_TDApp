using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class Rol
{
    public int RolId { get; set; }

    public string NombreRol { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
