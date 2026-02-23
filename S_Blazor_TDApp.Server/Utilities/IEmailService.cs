namespace S_Blazor_TDApp.Server.Utilities
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje);
    }
}
