using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class TareaCalendarioService(HttpClient httpClient) : ITareaCalendarioService
    {
        private readonly HttpClient _http = httpClient;

        public async Task<List<TareasCalendarioDTO>> Lista()
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<TareasCalendarioDTO>>>("api/TareasCalendario/Lista")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? [];
        }

        public async Task<TareasCalendarioDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<TareasCalendarioDTO>>($"api/TareasCalendario/Buscar/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<int> Guardar(TareasCalendarioDTO tareasCalendario)
        {
            // Se utiliza PostAsJsonAsync y se comprueba el código de estado HTTP.
            var httpResponse = await _http.PostAsJsonAsync("api/TareasCalendario/Guardar", tareasCalendario);
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

        public async Task<int> Editar(TareasCalendarioDTO tareasCalendario)
        {
            var httpResponse = await _http.PutAsJsonAsync($"api/TareasCalendario/Editar/{tareasCalendario.TareaId}", tareasCalendario);
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
            var httpResponse = await _http.DeleteAsync($"api/TareasCalendario/Eliminar/{id}");
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
    }
}