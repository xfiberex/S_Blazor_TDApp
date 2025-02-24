using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string? Email { get; set; }

    public int RolId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;
}
