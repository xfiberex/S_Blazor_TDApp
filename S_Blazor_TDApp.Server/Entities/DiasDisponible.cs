public partial class DiasDisponible
{
    public int DiaId { get; set; }

    public string NombreDia { get; set; } = null!;

    // Relación uno-a-muchos: un día puede estar en muchas filas de TareaDia
    public virtual ICollection<TareaDia> TareaDia { get; set; } = new List<TareaDia>();
}