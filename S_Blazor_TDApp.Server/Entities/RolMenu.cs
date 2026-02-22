namespace S_Blazor_TDApp.Server.Entities;

public partial class RolMenu
{
    public int RolId { get; set; }

    public int MenuId { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;
}
