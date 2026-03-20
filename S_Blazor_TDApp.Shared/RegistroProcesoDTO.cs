using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class RegistroProcesoDTO
    {
        public int ProcesoId { get; set; }

        public int TareaRecurrId { get; set; }

        public TareasRecurrentesDTO? RefTareaRecurr { get; set; }

        public int? TareaCalendarioId { get; set; }

        public TareasCalendarioDTO? RefTareaCalendario { get; set; }

        // Puede usarse para diferenciar en la vista
        public string TipoTarea { get; set; } = "Recurrente";

        public int UsuarioId { get; set; }

        public UsuarioDTO? RefUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(100, ErrorMessage = "La descripción de la tarea no puede exceder los 100 caracteres.")]
        public string DescripcionRegistro { get; set; } = null!;
    }
}