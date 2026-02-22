namespace S_Blazor_TDApp.Shared
{
    public class MenuDTO
    {
        public int MenuId { get; set; }

        public string NombreMenu { get; set; } = null!;

        public string Ruta { get; set; } = null!;

        public string Icono { get; set; } = null!;

        public string Seccion { get; set; } = null!;

        public int Orden { get; set; }
    }
}
