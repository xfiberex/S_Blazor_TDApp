using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class TareaDiasDTO
    {
        public int TareaDiaId { get; set; }

        [Required(ErrorMessage = "El ID de la tarea recurrente es obligatorio.")]
        public int TareaRecurrId { get; set; }

        [Required(ErrorMessage = "El día es obligatorio.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El día debe tener entre 3 y 20 caracteres.")]
        [RegularExpression("^(Lunes|Martes|Miércoles|Jueves|Viernes|Sábado|Domingo)$", ErrorMessage = "El día debe ser un día válido de la semana.")]
        public string Dia { get; set; } = null!;
    }
}