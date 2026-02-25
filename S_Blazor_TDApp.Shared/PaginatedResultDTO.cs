namespace S_Blazor_TDApp.Shared
{
    public class PaginatedResultDTO<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas => RegistrosPorPagina > 0
            ? (int)Math.Ceiling((double)TotalRegistros / RegistrosPorPagina)
            : 0;
    }
}
