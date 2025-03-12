using System.ComponentModel.DataAnnotations;

namespace S_Blazor_TDApp.Shared
{
    public class RegistroProcesoDTO
    {
        public int ProcesoId { get; set; }

        public int TareaRecurrId { get; set; }

        public TareasRecurrentesDTO? RefTareaRecurr { get; set; }

        public int UsuarioId { get; set; }

        public UsuarioDTO? RefUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El comentario debe tener entre 3 y 100 caracteres.")]
        public string DescripcionRegistro { get; set; } = null!;

        // TODO: Cronometrar las tareas recurrentes, para que expiren en un tiempo determinado.
        // TODO: Implementar logica para activar o desactivar las tareas recurrentes, de acuerdo a sus dias de disponibilidad.
        // TODO: Implementar inicio de sesíon, con roles y permisos, sin validación por correo.
        // TODO: Implementar logica para cambio de contraseña, cada 30, 60 o 90 días configurables, para los usuarios.
    }
}