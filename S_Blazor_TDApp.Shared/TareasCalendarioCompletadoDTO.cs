using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class TareasCalendarioCompletadoDTO
    {
        public int TareaCompletoId { get; set; }

        public int? TareaId { get; set; }

        public int? UsuarioId { get; set; }

        public bool EstadoCompletado { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, ErrorMessage = "La descripción de la tarea no puede exceder los 250 caracteres.")]
        public string DescripcionTareaCompletado { get; set; } = null!;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        public DateTime Hora { get; set; }

        public virtual TareasCalendarioDTO? RefTareaCalendario { get; set; }

    }
}
