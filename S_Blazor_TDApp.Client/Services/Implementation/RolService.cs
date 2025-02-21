using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class RolService : IRolService
    {
        private readonly HttpClient _http;

        public RolService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<List<RolDTO>> Lista()
        {
            // Llamar a la API para recuperar la lista de roles
            var resultado = await _http.GetFromJsonAsync<ResponseAPI<List<RolDTO>>>("api/Rol/Lista");

            // Verificar si la respuesta es correcta
            if (resultado!.EsCorrecto)
            {
                return resultado.Valor!;
            }
            else
            {
                throw new Exception(resultado.Mensaje);
            }
        }
    }
}