using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Contexto de la base de datos
builder.Services.AddDbContext<DbTdappContext>(options =>
{
    // Conexion a la base de datos del equipo A
    //options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLPrimary"));

    // Conexion a la base de datos del equipo B
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLSecundary"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("nuevaPolitica", app =>
    {
        app.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("nuevaPolitica");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();