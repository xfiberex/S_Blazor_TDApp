using Blazored.SessionStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using S_Blazor_TDApp.Client;
using S_Blazor_TDApp.Client.Services;
using S_Blazor_TDApp.Client.Extensions;
using S_Blazor_TDApp.Client.Services.Implementation;
using S_Blazor_TDApp.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuraci�n para utilizar HTTPS:
builder.Services.AddTransient<JwtAuthenticationInterceptor>();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7219/");
}).AddHttpMessageHandler<JwtAuthenticationInterceptor>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Servicios de la aplicaci�n para la gestion de datos
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITareaCalendarioService, TareaCalendarioService>();
builder.Services.AddScoped<ITareaRecurrenteService, TareaRecurrenteService>();
builder.Services.AddScoped<IRegistroProcesosService, RegistroProcesosService>();
builder.Services.AddScoped<ITareaDiasService, TareaDiasService>();
builder.Services.AddScoped<IMenuPermisoService, MenuPermisoService>();
builder.Services.AddScoped<MenuPermisoEstado>();

// Servicios para la autenticaci�n de usuarios y roles
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AutenticacionExtension>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AutenticacionExtension>());
builder.Services.AddAuthorizationCore();

// Servicios de la aplicaci�n para la gestion de la interfaz
builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();