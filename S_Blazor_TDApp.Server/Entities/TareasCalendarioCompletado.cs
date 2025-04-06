namespace S_Blazor_TDApp.Server.Entities;

public partial class TareasCalendarioCompletado
{
    public int TareaCompletoId { get; set; }

    public int? TareaId { get; set; }

    public int? UsuarioId { get; set; }

    public bool EstadoCompletado { get; set; }

    public string? DescripcionTareaCompletado { get; set; }

    public DateTime Fecha { get; set; }

    public virtual TareasCalendario? RefTarea { get; set; }

    public virtual Usuario? RefUsuario { get; set; }
}
