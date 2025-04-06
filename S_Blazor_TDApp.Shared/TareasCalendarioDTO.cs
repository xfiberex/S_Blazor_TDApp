using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class TareasCalendarioDTO
    {
        public int TareaId { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de la tarea debe tener entre 3 y 100 caracteres.")]
        public string NombreTarea { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250, ErrorMessage = "La descripción de la tarea no puede exceder los 250 caracteres.")]
        public string? DescripcionTarea { get; set; }

        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La hora es obligatoria")]
        public DateTime Hora { get; set; } = DateTime.Now;

        // Nueva propiedad para controlar la visibilidad de la descripción completa
        public bool ShowFullDescription { get; set; } = false;

        public TareasCalendarioCompletadoDTO? ReftareasCalendarioCompletado { get; set; }
    }
}