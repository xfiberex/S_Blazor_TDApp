using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using S_Blazor_TDApp.Client;
using S_Blazor_TDApp.Client.Services.Implementation;
using S_Blazor_TDApp.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7219/") });

// Add services to the container.
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITareaCalendarioService, TareaCalendarioService>();
builder.Services.AddScoped<ITareaRecurrenteService, TareaRecurrenteService>();
builder.Services.AddScoped<ITareaDiasService, TareaDiasService>();

builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();