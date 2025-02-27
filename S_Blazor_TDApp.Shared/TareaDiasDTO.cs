namespace S_Blazor_TDApp.Shared
{
    public class TareaDiasDTO
    {
        public int TareaDiaId { get; set; }

        public int TareaRecurrId { get; set; }

        // Objeto anidado para exponer la info del día (id + nombre)
        public DiasDisponiblesDTO Dia { get; set; } = null!;
    }
}