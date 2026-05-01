using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class UsuarioService(HttpClient httpClient) : IUsuarioService
    {
        private readonly HttpClient _http = httpClient;

        public async Task<List<UsuarioDTO>> Lista()
        {
            var httpResponse = await _http.GetAsync("api/usuarios");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<PaginatedResultDTO<UsuarioDTO>>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor?.Items ?? [];
        }

        public async Task<InicioSesionDTO> Login(LoginDTO login)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios/login", login);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<InicioSesionDTO>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor!;
        }

        public async Task<bool> Registro(RegistroUsuarioDTO registro)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios/registro", registro);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<string> ConfirmarCorreo(string token, string email)
        {
            var httpResponse = await _http.GetAsync($"api/usuarios/confirmar-correo?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}");
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>();
            return response?.Mensaje ?? "Correo confirmado exitosamente.";
        }

        public async Task<bool> OlvideContrasena(OlvideContrasenaDTO request)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios/olvide-contrasena", request);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<bool> RestablecerContrasena(RestablecerContrasenaDTO request)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios/restablecer-contrasena", request);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<PerfilUsuarioDTO> ObtenerPerfil()
        {
            var httpResponse = await _http.GetAsync("api/usuarios/perfil");
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<PerfilUsuarioDTO>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor!;
        }

        public async Task<bool> ActualizarPerfil(PerfilUsuarioDTO perfil)
        {
            var httpResponse = await _http.PutAsJsonAsync("api/usuarios/perfil", perfil);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<bool> CambiarContrasenaPerfil(CambiarContrasenaPerfilDTO request)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios/cambiar-contrasena", request);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<InicioSesionDTO> RefreshToken()
        {
            var httpResponse = await _http.PostAsync("api/usuarios/refresh-token", null);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<InicioSesionDTO>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor!;
        }

        public async Task<bool> RevocarToken()
        {
            var httpResponse = await _http.PostAsync("api/usuarios/revocar-token", null);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<UsuarioDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<UsuarioDTO>>($"api/usuarios/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<bool> ExisteCodigo(string codigo)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<bool>>($"api/usuarios/existe-codigo/{codigo}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<UsuarioDTO?> ObtenerPorEmail(string email)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<UsuarioDTO>>($"api/usuarios/por-email?email={Uri.EscapeDataString(email)}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<int> Guardar(UsuarioDTO usuario)
        {
            // Se utiliza PostAsJsonAsync y se comprueba el código de estado HTTP.
            var httpResponse = await _http.PostAsJsonAsync("api/usuarios", usuario);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<int>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor!;
        }

        public async Task<int> Editar(UsuarioDTO usuario)
        {
            var httpResponse = await _http.PutAsJsonAsync($"api/usuarios/{usuario.UsuarioId}", usuario);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<int>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor!;
        }

        public async Task<bool> Eliminar(int id)
        {
            var httpResponse = await _http.DeleteAsync($"api/usuarios/{id}");
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<int>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            // Retorna true en caso de éxito
            return true;
        }

        public async Task CambiarClave(int usuarioId, CambioClaveDTO cambioClaveDto)
        {
            var httpResponse = await _http.PutAsJsonAsync($"api/usuarios/cambiar-clave/{usuarioId}", cambioClaveDto);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }
        }
    }
}