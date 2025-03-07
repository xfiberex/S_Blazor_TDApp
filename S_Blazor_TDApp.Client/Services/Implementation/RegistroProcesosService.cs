using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class RegistroProcesosService(HttpClient httpClient) : IRegistroProcesosService
    {
        private readonly HttpClient _http = httpClient;

        public async Task<List<TareasRecurrentesDTO>> Lista()
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<TareasRecurrentesDTO>>>("api/TareasRecurrentes/Lista")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? [];
        }

        public async Task<List<RegistroProcesoDTO>> ListaProcesos()
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<RegistroProcesoDTO>>>("api/ProcesosRegistro/ListaProcesos")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor ?? [];
        }

        public async Task<TareasRecurrentesDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<TareasRecurrentesDTO>>($"api/ProcesosRegistro/Buscar/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<RegistroProcesoDTO> BuscarProcesos(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<RegistroProcesoDTO>>($"api/ProcesosRegistro/BuscarProcesos/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<int> GuardarProcesos(RegistroProcesoDTO proceso)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/ProcesosRegistro/GuardarProcesos", proceso);
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
    }
}