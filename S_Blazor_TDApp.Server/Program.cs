using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Utilities.AutoMapper;
using S_Blazor_TDApp.Server.Utilities.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Registrar AutoMapper
builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

// Registrar el servicio en segundo plano
builder.Services.AddHostedService<TareaExpiracionService>();

// Contexto de base de datos
builder.Services.AddDbContext<DbTdappContext>(options =>
{
    /* Para utilizar dos equipos SQL Server, puedes descomentar la línea correspondiente,
       de acuerdo a tu configuración en el archivo appsettings.json */

    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLPrimary"));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLSecundary"));
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