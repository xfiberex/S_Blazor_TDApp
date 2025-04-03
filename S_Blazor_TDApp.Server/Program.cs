using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Utilities.AutoMapper;
using S_Blazor_TDApp.Server.Utilities.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registrar el servicio en segundo plano
builder.Services.AddHostedService<TareaExpiracionService>();

// Contexto de base de datos
builder.Services.AddDbContext<DbTdappContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLPrimary"));
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLSecundary"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("nuevaPolitica", app =>
    {
        app.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Habilitar Swagger y la UI solo en Desarrollo (o según prefieras)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("nuevaPolitica");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();