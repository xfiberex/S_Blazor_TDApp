using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Services.Interfaces;
using S_Blazor_TDApp.Server.Services.Implementation;
using S_Blazor_TDApp.Server.Utilities.AutoMapper;
using S_Blazor_TDApp.Server.Utilities.BackgroundServices;
using S_Blazor_TDApp.Server.Utilities;

const string AccessTokenCookieName = "tdapp.access_token";

var builder = WebApplication.CreateBuilder(args);

// appsettings.Local.json sobreescribe cualquier valor de appsettings.json (está en .gitignore)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

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

// Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("La clave JWT ('Jwt:Key') no está configurada. Use dotnet user-secrets o variables de entorno.");

var connectionString = builder.Configuration.GetConnectionString("cadenaSQLPrimary");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("La cadena de conexión 'ConnectionStrings:cadenaSQLPrimary' no está configurada. Use appsettings.Local.json, user-secrets o variables de entorno.");

var key = Encoding.UTF8.GetBytes(jwtKey);

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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrWhiteSpace(context.Token) &&
                context.Request.Cookies.TryGetValue(AccessTokenCookieName, out var accessToken))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// Registrar AutoMapper
builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());

// Registrar el servicio en segundo plano
builder.Services.AddHostedService<TareaExpiracionService>();

// Registrar EmailService
builder.Services.AddScoped<IEmailService, EmailService>();

// Registrar TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// Contexto de base de datos
builder.Services.AddDbContext<DbTdappContext>(options =>
{
    /* Para utilizar dos equipos SQL Server, puedes descomentar la línea correspondiente,
       de acuerdo a tu configuraci�n en el archivo appsettings.json */

    options.UseSqlServer(connectionString);
    //options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQLSecundary"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("nuevaPolitica", app =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (allowedOrigins.Length == 0)
        {
            return;
        }

        app.WithOrigins(allowedOrigins)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
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

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    await next();
});

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