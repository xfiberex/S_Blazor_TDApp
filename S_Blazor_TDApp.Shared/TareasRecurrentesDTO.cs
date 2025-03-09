using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class TareasRecurrentesDTO
    {
        public int TareaRecurrId { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea recurrente es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de la tarea recurrente debe tener entre 3 y 100 caracteres.")]
        public string NombreTareaRecurr { get; set; } = null!;

        [Required(ErrorMessage = "La descripción de la tarea recurrente es obligatoria.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "La descripción de la tarea debe tener entre 3 y 100 caracteres.")]
        public string DescripcionTareaRecurr { get; set; } = null!;

        [Required(ErrorMessage = "Seleccione si la tarea es recurrente.")]
        public bool Recurrente { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        public DateTime HoraDesde { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        public DateTime HorasHasta { get; set; } = DateTime.Now;

        [Range(1, int.MaxValue, ErrorMessage = "El tiempo de ejecución debe ser mayor a 0.")]
        public int TiempoEjecucion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de ejecuciones debe ser mayor a 0.")]
        public int CantidadEjecuciones { get; set; }

        public bool Estado { get; set; }

        // Nueva propiedad para controlar la visibilidad de la descripción completa
        public bool ShowFullDescription { get; set; } = false;
    }
}