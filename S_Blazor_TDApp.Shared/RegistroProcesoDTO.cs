using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Blazor_TDApp.Shared
{
    public class RegistroProcesoDTO
    {
        public int ProcesoId { get; set; }

        public int TareaRecurrId { get; set; }

        public int UsuarioId { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El comentario debe tener entre 3 y 100 caracteres.")]
        public string Comentario { get; set; } = null!;
    }
}