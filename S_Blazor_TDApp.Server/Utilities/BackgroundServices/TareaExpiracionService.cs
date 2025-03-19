using Microsoft.EntityFrameworkCore;
using S_Blazor_TDApp.Server.DBContext;

namespace S_Blazor_TDApp.Server.Utilities.BackgroundServices
{
    public class TareaExpiracionService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TareaExpiracionService> _logger;

        // Intervalo de revisión, por ejemplo, cada 30 segundos.
        private readonly TimeSpan _revisarIntervalo = TimeSpan.FromSeconds(30);

        public TareaExpiracionService(IServiceScopeFactory scopeFactory, ILogger<TareaExpiracionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DbTdappContext>();

                        // Se obtiene la lista de tareas activas de forma asíncrona
                        var tareas = await context.TareasRecurrentes
                            .Where(t => t.Estado)
                            .ToListAsync(stoppingToken);

                        foreach (var tarea in tareas)
                        {
                            // Calcula el tiempo transcurrido desde la última renovación.
                            var tiempoTranscurrido = DateTime.Now - tarea.FechaUltimaRenovacion;

                            // Se asume que "TiempoEjecucion" está en minutos
                            if (tiempoTranscurrido.TotalMinutes >= tarea.TiempoEjecucion)
                            {
                                // Según la lógica de negocio, se puede:
                                // O marcar la tarea como expirada (sin renovar automáticamente):
                                tarea.EstadoExpiracion = false;
                                _logger.LogInformation($"La tarea {tarea.TareaRecurrId} ha expirado.");

                                // O, si se desea renovar automáticamente:
                                // tarea.EstadoExpiracion = true;
                                // tarea.FechaUltimaRenovacion = DateTime.Now;
                                // _logger.LogInformation($"La tarea {tarea.TareaRecurrId} se ha renovado.");
                            }
                        }
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al verificar expiración de tareas.");
                }
                await Task.Delay(_revisarIntervalo, stoppingToken);
            }
        }
    }
}