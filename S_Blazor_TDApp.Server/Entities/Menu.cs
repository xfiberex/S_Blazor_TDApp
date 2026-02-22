namespace S_Blazor_TDApp.Server.Entities;

public partial class Menu
{
    public int MenuId { get; set; }

    public string NombreMenu { get; set; } = null!;

    public string Ruta { get; set; } = null!;

    public string Icono { get; set; } = null!;

    public string Seccion { get; set; } = null!;

    public int Orden { get; set; }

    public virtual ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();
}
