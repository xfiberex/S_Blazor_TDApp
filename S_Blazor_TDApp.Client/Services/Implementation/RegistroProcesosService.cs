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
            var httpResponse = await _http.GetAsync("api/tareas-recurrentes");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<PaginatedResultDTO<TareasRecurrentesDTO>>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor?.Items ?? new List<TareasRecurrentesDTO>();
        }

        public async Task<List<RegistroProcesoDTO>> ListaProcesos()
        {
            var httpResponse = await _http.GetAsync("api/procesos-registro");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<PaginatedResultDTO<RegistroProcesoDTO>>>()
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor?.Items ?? new List<RegistroProcesoDTO>();
        }

        public async Task<TareasRecurrentesDTO> Buscar(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<TareasRecurrentesDTO>>($"api/tareas-recurrentes/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<RegistroProcesoDTO> BuscarProcesos(int id)
        {
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<RegistroProcesoDTO>>($"api/procesos-registro/{id}")
                ?? throw new Exception("No se recibió respuesta del servidor.");

            if (!resultado.EsCorrecto)
                throw new Exception(resultado.Mensaje);

            return resultado.Valor!;
        }

        public async Task<int> GuardarProcesos(RegistroProcesoDTO proceso)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/procesos-registro", proceso);
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

        public async Task<int> RegistrarTareaCalendario(TareasCalendarioCompletadoDTO calendarioDTO)
        {
            var httpResponse = await _http.PostAsJsonAsync("api/procesos-registro/calendario-completado", calendarioDTO);
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