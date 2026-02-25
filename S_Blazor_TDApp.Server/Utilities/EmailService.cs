using System.Net;
using System.Net.Mail;

namespace S_Blazor_TDApp.Server.Utilities
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
        {
            try
            {
                var host = _configuration["Smtp:Host"]
                    ?? throw new InvalidOperationException("Smtp:Host no configurado.");
                var port = int.Parse(_configuration["Smtp:Port"]
                    ?? throw new InvalidOperationException("Smtp:Port no configurado."));
                var user = _configuration["Smtp:User"]
                    ?? throw new InvalidOperationException("Smtp:User no configurado.");
                var pass = _configuration["Smtp:Pass"]
                    ?? throw new InvalidOperationException("Smtp:Pass no configurado.");
                var from = _configuration["Smtp:From"]
                    ?? throw new InvalidOperationException("Smtp:From no configurado.");

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(destinatario);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado a {Destinatario}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {Destinatario}", destinatario);
                // No lanzamos la excepción para no bloquear el flujo principal si falla el correo
            }
        }
    }
}
