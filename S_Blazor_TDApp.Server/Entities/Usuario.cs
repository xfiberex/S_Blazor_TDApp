using System;
using System.Collections.Generic;

namespace S_Blazor_TDApp.Server.Entities;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Codigo { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string? Email { get; set; }

    public int RolId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual ICollection<RegistroProceso> RegistroProcesos { get; set; } = new List<RegistroProceso>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<TareasCalendarioCompletado> TareasCalendarioCompletados { get; set; } = new List<TareasCalendarioCompletado>();
}
