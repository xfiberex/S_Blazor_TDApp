using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Blazor_TDApp.Shared
{
    public class TareasCalendarioCompletadoDTO
    {
        public int TareaCompletoId { get; set; }

        public int? TareaId { get; set; }

        public int? UsuarioId { get; set; }

        public bool EstadoCompletado { get; set; }

        public string DescripcionTareaCompletado { get; set; } = null!;

        public DateTime Fecha_Hora { get; set; }

        public virtual TareasCalendarioDTO? RefTareaCalendario { get; set; }
        
    }
}
