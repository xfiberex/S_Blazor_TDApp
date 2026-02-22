namespace S_Blazor_TDApp.Shared
{
    /// <summary>
    /// DTO para actualizar los permisos de un rol.
    /// Contiene el RolId y la lista de MenuIds a los que tendrá acceso.
    /// </summary>
    public class ActualizarPermisosDTO
    {
        public int RolId { get; set; }

        /// <summary>
        /// IDs de los menús que este rol podrá ver.
        /// Los menús no incluidos en la lista perderán el acceso.
        /// </summary>
        public List<int> MenuIds { get; set; } = new List<int>();
    }
}
