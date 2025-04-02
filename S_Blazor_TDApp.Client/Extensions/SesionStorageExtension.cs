using Blazored.SessionStorage;
using System.Text.Json;

namespace S_Blazor_TDApp.Client.Extensions
{
    public static class SesionStorageExtension
    {
        /// <summary>
        /// Guarda un objeto en el almacenamiento de sesión como una cadena JSON.
        /// </summary>
        /// <typeparam name="T">El tipo del objeto a almacenar. Debe ser una clase.</typeparam>
        /// <param name="sessionStorageService">La instancia de ISessionStorageService para interactuar con el almacenamiento de sesión.</param>
        /// <param name="key">La clave bajo la cual se almacenará el objeto.</param>
        /// <param name="item">El objeto que se va a almacenar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public static async Task GuardarStorage<T>(
            this ISessionStorageService sessionStorageService,
            string key, T item
            ) where T : class
        {
            var itemJson = JsonSerializer.Serialize(item);
            await sessionStorageService.SetItemAsStringAsync(key, itemJson);
        }

        /// <summary>
        /// Obtiene un objeto del almacenamiento de sesión deserializándolo desde una cadena JSON.
        /// </summary>
        /// <typeparam name="T">El tipo del objeto a obtener. Debe ser una clase.</typeparam>
        /// <param name="sessionStorageService">La instancia de ISessionStorageService para interactuar con el almacenamiento de sesión.</param>
        /// <param name="key">La clave bajo la cual se almacenó el objeto.</param>
        /// <returns>El objeto deserializado o null si no se encuentra.</returns>
        public static async Task<T?> ObtenerStorage<T>(
        this ISessionStorageService sessionStorageService,
        string key
        ) where T : class
        {
            var itemJson = await sessionStorageService.GetItemAsStringAsync(key);

            if (itemJson != null)
            {
                var item = JsonSerializer.Deserialize<T>(itemJson);
                return item;
            }
            else
                return null;
        }
    }
}