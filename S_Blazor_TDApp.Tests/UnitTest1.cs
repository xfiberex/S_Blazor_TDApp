using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using S_Blazor_TDApp.Server.Controllers;
using S_Blazor_TDApp.Server.DBContext;
using S_Blazor_TDApp.Server.Entities;
using S_Blazor_TDApp.Server.Services.Implementation;
using S_Blazor_TDApp.Server.Utilities;
using S_Blazor_TDApp.Shared;

namespace S_Blazor_TDApp.Tests;

public class AuthenticationSecurityTests
{
    [Fact]
    public void TokenHashHelper_MatchesHashedAndLegacyTokens()
    {
        const string rawToken = "token-seguro";
        var hashedToken = TokenHashHelper.HashToken(rawToken);

        Assert.True(TokenHashHelper.Matches(rawToken, hashedToken));
        Assert.True(TokenHashHelper.Matches(rawToken, rawToken));
        Assert.False(TokenHashHelper.Matches("otro-token", hashedToken));
    }

    [Fact]
    public async Task Login_StoresHashedRefreshToken_AndSetsAuthCookies()
    {
        await using var context = CreateContext();
        var role = CreateRole();
        var user = CreateUser(role, email: "login@test.com");

        context.Roles.Add(role);
        context.Usuarios.Add(user);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Login(new LoginDTO
        {
            Email = user.Email!,
            Clave = "Secret123!"
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ResponseAPI<InicioSesionDTO>>(okResult.Value);

        Assert.True(payload.EsCorrecto);
        Assert.NotNull(payload.Valor);
        Assert.Equal(user.UsuarioId, payload.Valor.UsuarioId);

        var storedUser = await context.Usuarios.SingleAsync();
        Assert.NotNull(storedUser.RefreshToken);
        Assert.Equal(64, storedUser.RefreshToken!.Length);
        Assert.NotNull(storedUser.RefreshTokenExpiracion);

        var setCookieHeader = controller.HttpContext.Response.Headers.SetCookie.ToString();
        Assert.Contains("tdapp.access_token=", setCookieHeader, StringComparison.Ordinal);
        Assert.Contains("tdapp.refresh_token=", setCookieHeader, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RefreshToken_RotatesStoredHash_WhenCookieIsValid()
    {
        const string initialRefreshToken = "refresh-token-inicial";

        await using var context = CreateContext();
        var role = CreateRole();
        var user = CreateUser(role, email: "refresh@test.com");
        user.RefreshToken = TokenHashHelper.HashToken(initialRefreshToken);
        user.RefreshTokenExpiracion = DateTime.UtcNow.AddMinutes(30);

        context.Roles.Add(role);
        context.Usuarios.Add(user);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        controller.HttpContext.Request.Headers.Cookie = $"tdapp.refresh_token={initialRefreshToken}";

        var result = await controller.RefreshToken();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ResponseAPI<InicioSesionDTO>>(okResult.Value);

        Assert.True(payload.EsCorrecto);
        Assert.NotNull(payload.Valor);

        var updatedUser = await context.Usuarios.SingleAsync();
        Assert.NotNull(updatedUser.RefreshToken);
        Assert.NotEqual(TokenHashHelper.HashToken(initialRefreshToken), updatedUser.RefreshToken);
        Assert.Equal(64, updatedUser.RefreshToken!.Length);

        var setCookieHeader = controller.HttpContext.Response.Headers.SetCookie.ToString();
        Assert.Contains("tdapp.access_token=", setCookieHeader, StringComparison.Ordinal);
        Assert.Contains("tdapp.refresh_token=", setCookieHeader, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConfirmarCorreo_ReturnsBadRequest_WhenTokenIsExpired()
    {
        const string confirmationToken = "confirm-token-expired";

        await using var context = CreateContext();
        var role = CreateRole();
        var user = CreateUser(role, email: "confirm@test.com");
        user.CorreoConfirmado = false;
        user.TokenConfirmacion = TokenHashHelper.HashToken(confirmationToken);
        user.FechaExpiracionToken = DateTime.UtcNow.AddMinutes(-5);

        context.Roles.Add(role);
        context.Usuarios.Add(user);
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.ConfirmarCorreo(confirmationToken, user.Email!, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ResponseAPI<bool>>(badRequest.Value);

        Assert.False(payload.EsCorrecto);
        Assert.Equal("Token o correo inválido.", payload.Mensaje);
    }

    private static DbTdappContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DbTdappContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DbTdappContext(options);
    }

    private static UsuarioController CreateController(DbTdappContext context)
    {
        var mapper = new MapperConfiguration(_ => { }, NullLoggerFactory.Instance).CreateMapper();
        var configuration = BuildConfiguration();
        var tokenService = new TokenService(configuration);

        var controller = new UsuarioController(
            context,
            mapper,
            tokenService,
            new NullEmailService(),
            NullLogger<UsuarioController>.Instance,
            configuration,
            new TestWebHostEnvironment());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                ["Jwt:Issuer"] = "S_Blazor_TDApp.Tests",
                ["Jwt:Audience"] = "S_Blazor_TDApp.Tests.Client",
                ["ClientApp:BaseUrl"] = "https://localhost:7041/"
            })
            .Build();
    }

    private static Rol CreateRole()
    {
        return new Rol
        {
            RolId = 1,
            NombreRol = "Administrador",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    private static Usuario CreateUser(Rol role, string email)
    {
        return new Usuario
        {
            UsuarioId = Random.Shared.Next(1, int.MaxValue),
            Codigo = Random.Shared.Next(10000, 99999).ToString(),
            NombreUsuario = email.Split('@')[0],
            NombreCompleto = "Usuario de Prueba",
            Clave = PasswordHelper.HashPassword("Secret123!"),
            Email = email,
            CorreoConfirmado = true,
            RolId = role.RolId,
            IdRolNavigation = role,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };
    }

    private sealed class NullEmailService : IEmailService
    {
        public Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "S_Blazor_TDApp.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Development";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
