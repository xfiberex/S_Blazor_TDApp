using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Utilities.AutoMapper;
using S_Blazor_TDApp.Server.Utilities.BackgroundServices;
using S_Blazor_TDApp.Server.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configuraci�n de JWT
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

// Registrar EmailService
builder.Services.AddScoped<IEmailService, EmailService>();

// Contexto de base de datos
builder.Services.AddDbContext<DbTdappContext>(options =>
{
    /* Para utilizar dos equipos SQL Server, puedes descomentar la l�nea correspondiente,
       de acuerdo a tu configuraci�n en el archivo appsettings.json */

    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLPrimary"));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLSecundary"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("nuevaPolitica", app =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        app.WithOrigins(allowedOrigins)
           .AllowAnyMethod()
           .AllowAnyHeader();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();
app.UseExceptionHandler();
// Habilitar Swagger y la UI solo en Desarrollo (o seg�n prefieras)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("nuevaPolitica");
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();