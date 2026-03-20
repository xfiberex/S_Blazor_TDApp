using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using S_Blazor_TDApp.Shared;
using S_Blazor_TDApp.Client.Services.Interfaces;
using CurrieTechnologies.Razor.SweetAlert2;

namespace S_Blazor_TDApp.Client.Pages
{
    public partial class Usuarios : ComponentBase
    {
        [Inject] public IUsuarioService UsuarioService { get; set; } = null!;
        [Inject] public SweetAlertService SwalService { get; set; } = null!;
        [Inject] public NavigationManager Navigation { get; set; } = null!;
        [Inject] public AuthenticationStateProvider authStateProvider { get; set; } = null!;

        private List<UsuarioDTO> listaUsuarios = new();
        private List<UsuarioDTO> filteredUsers = new();
        private List<UsuarioDTO> pagedUsers = new();

        private bool isLoading = true;
        private string searchTerm = "";

        // Parámetros de paginación
        private int currentPage = 1;
        private int pageSize = 10;
        private readonly int[] pageSizeOptions = { 10, 15, 20 };

        // Timer para el debounce en la búsqueda
        private System.Threading.Timer? _searchTimer;

        // Variable para almacenar el ID y rol del usuario autenticado
        private int currentUserId;
        private string currentRol = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await ObtenerUsuarioAutenticado();
            await LoadUsuarios();
        }

        private async Task ObtenerUsuarioAutenticado()
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(idClaim))
                    currentUserId = int.Parse(idClaim);

                currentRol = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
            }
        }

        private async Task LoadUsuarios()
        {
            try
            {
                isLoading = true;
                listaUsuarios = await UsuarioService.Lista();
                filteredUsers = listaUsuarios;
                currentPage = 1;
                UpdatePagedUsers();
            }
            catch (UnauthorizedAccessException)
            {
                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {
                await SwalService.FireAsync(new SweetAlertOptions
                {
                    Title = "Error",
                    Text = $"Error al cargar usuarios: {ex.Message}",
                    Icon = SweetAlertIcon.Error
                });
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task Eliminar(int id, string nombreUsuario)
        {
            // Guardia: verificar permisos antes de proceder
            var target = listaUsuarios.FirstOrDefault(u => u.UsuarioId == id);
            if (target == null || !PuedeModificar(target)) return;
            var result = await SwalService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar el usuario {nombreUsuario}?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    if (await UsuarioService.Eliminar(id))
                    {
                        listaUsuarios.RemoveAll(u => u.UsuarioId == id);
                        FilterUsers();

                        await SwalService.FireAsync(new SweetAlertOptions
                        {
                            Title = "Eliminado",
                            Text = "El usuario ha sido eliminado correctamente.",
                            Icon = SweetAlertIcon.Success
                        });
                    }
                }
                catch (Exception ex)
                {
                    await SwalService.FireAsync(new SweetAlertOptions
                    {
                        Title = "Error",
                        Text = $"Ha ocurrido un error al eliminar el usuario: {ex.Message}",
                        Icon = SweetAlertIcon.Error
                    });
                }
            }
        }

        private void OnSearchTermChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            _searchTimer?.Dispose();
            _searchTimer = new System.Threading.Timer(_ =>
            {
                InvokeAsync(() =>
                {
                    FilterUsers();
                    StateHasChanged();
                });
            }, null, 500, System.Threading.Timeout.Infinite);
        }

        private void FilterUsers()
        {
            filteredUsers = string.IsNullOrWhiteSpace(searchTerm)
                ? listaUsuarios
                : listaUsuarios.Where(u =>
                     (u.Codigo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                      u.NombreUsuario.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                      u.NombreCompleto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                     (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                     (u.Rol?.NombreRol?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                ).ToList();

            currentPage = 1;
            UpdatePagedUsers();
        }

        private void ClearSearch()
        {
            searchTerm = string.Empty;
            FilterUsers();
        }

        private void UpdatePagedUsers()
        {
            pagedUsers = filteredUsers
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int newPageSize))
            {
                pageSize = newPageSize;
                currentPage = 1;
                UpdatePagedUsers();
            }
        }

        private void NextPage()
        {
            if (currentPage * pageSize < filteredUsers.Count)
            {
                currentPage++;
                UpdatePagedUsers();
            }
        }

        private void PrevPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagedUsers();
            }
        }

        private static int GetRolPrioridad(string? rolNombre) => rolNombre switch
        {
            "Super_Administrador" => 100,
            "Administrador" => 50,
            "Supervisor" => 20,
            "Empleado" => 10,
            _ => 0
        };

        private bool PuedeModificar(UsuarioDTO item)
        {
            if (item.UsuarioId == currentUserId) return false;
            return GetRolPrioridad(currentRol) > GetRolPrioridad(item.Rol?.NombreRol);
        }
    }
}