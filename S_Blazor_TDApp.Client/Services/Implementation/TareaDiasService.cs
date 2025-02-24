using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class TareaDiasService(HttpClient httpClient) : ITareaDiasService
    {
        private readonly HttpClient _http = httpClient;

        public async Task<List<TareaDiasDTO>> Lista()
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<TareaDiasDTO>>>("api/TareaDias/Lista")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? [];
        }

        public async Task<List<TareaDiasDTO>> ListaPorTareaRecurrId(int tareaRecurrId)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<TareaDiasDTO>>>($"api/TareaDias/ListaPorTarea?tareaRecurrId={tareaRecurrId}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? new List<TareaDiasDTO>();
        }

        public async Task<TareaDiasDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<TareaDiasDTO>>($"api/TareaDias/Buscar/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<int> Guardar(TareaDiasDTO tareaDias)
        {
            // Se utiliza PostAsJsonAsync y se comprueba el código de estado HTTP.
            var httpResponse = await _http.PostAsJsonAsync("api/TareaDias/Guardar", tareaDias);
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

        public async Task<int> Editar(TareaDiasDTO tareaDias)
        {
            var httpResponse = await _http.PutAsJsonAsync($"api/TareaDias/Editar/{tareaDias.TareaRecurrId}", tareaDias);
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
            var httpResponse = await _http.DeleteAsync($"api/TareaDias/Eliminar/{id}");
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