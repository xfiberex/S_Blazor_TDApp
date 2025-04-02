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
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<UsuarioDTO>>>("api/Usuario/Lista")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? [];
        }

        public async Task<UsuarioDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<UsuarioDTO>>($"api/Usuario/Buscar/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<bool> ExisteCodigo(string codigo)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<bool>>($"api/Usuario/ExisteCodigo/{codigo}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<UsuarioDTO?> ObtenerPorEmail(string email)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<UsuarioDTO>>($"api/Usuario/ObtenerPorEmail/{email}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!response.EsCorrecto)
                throw new Exception(response.Mensaje);

            return response.Valor;
        }

        public async Task<int> Guardar(UsuarioDTO usuario)
        {
            // Se utiliza PostAsJsonAsync y se comprueba el código de estado HTTP.
            var httpResponse = await _http.PostAsJsonAsync("api/Usuario/Guardar", usuario);
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
            var httpResponse = await _http.PutAsJsonAsync($"api/Usuario/Editar/{usuario.UsuarioId}", usuario);
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
            var httpResponse = await _http.DeleteAsync($"api/Usuario/Eliminar/{id}");
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
            var httpResponse = await _http.PutAsJsonAsync($"api/Usuario/CambiarClave/{usuarioId}", cambioClaveDto);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error en la llamada API: {httpResponse.ReasonPhrase} - {errorContent}");
            }
        }
    }
}