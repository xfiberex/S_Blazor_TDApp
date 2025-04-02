using Blazored.SessionStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using S_Blazor_TDApp.Client;
using S_Blazor_TDApp.Client.Extensions;
using S_Blazor_TDApp.Client.Services.Implementation;
using S_Blazor_TDApp.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuración para utilizar HTTPS:
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7219/") });

// Servicios de la aplicación para la gestion de datos
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITareaCalendarioService, TareaCalendarioService>();
builder.Services.AddScoped<ITareaRecurrenteService, TareaRecurrenteService>();
builder.Services.AddScoped<IRegistroProcesosService, RegistroProcesosService>();
builder.Services.AddScoped<ITareaDiasService, TareaDiasService>();

// Servicios para la autenticación de usuarios y roles
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, AutenticacionExtension>();
builder.Services.AddAuthorizationCore();

// Servicios de la aplicación para la gestion de la interfaz
builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();