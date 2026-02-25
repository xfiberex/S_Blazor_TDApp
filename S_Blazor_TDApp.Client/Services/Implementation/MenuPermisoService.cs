using S_Blazor_TDApp.Client.Services.Interfaces;
using S_Blazor_TDApp.Shared;
using System.Net.Http.Json;

namespace S_Blazor_TDApp.Client.Services.Implementation
{
    public class MenuPermisoService : IMenuPermisoService
    {
        private readonly HttpClient _http;

        public MenuPermisoService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<List<MenuDTO>> TodosLosMenus()
        {
            var httpResponse = await _http.GetAsync("api/menus");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<List<MenuDTO>>>();
            if (resultado!.EsCorrecto)
                return resultado.Valor!;

            throw new Exception(resultado.Mensaje);
        }

        public async Task<List<MenuDTO>> MenusPorRol(int rolId)
        {
            var httpResponse = await _http.GetAsync($"api/menus/por-rol/{rolId}");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<List<MenuDTO>>>();
            if (resultado!.EsCorrecto)
                return resultado.Valor!;

            throw new Exception(resultado.Mensaje);
        }

        public async Task<bool> ActualizarPermisos(ActualizarPermisosDTO dto)
        {
            var httpResponse = await _http.PutAsJsonAsync("api/menus/permisos", dto);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar permisos: {httpResponse.ReasonPhrase} - {errorContent}");
            }

            var resultado = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<bool>>();
            if (resultado!.EsCorrecto)
                return resultado.Valor;

            throw new Exception(resultado.Mensaje);
        }
    }
}
