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
                        // Obtiene las tareas activas
                        var tareas = context.TareasRecurrentes
                            .Where(t => t.Estado == true)
                            .ToList();

                        foreach (var tarea in tareas)
                        {
                            // Calcula el tiempo transcurrido desde la última renovación.
                            var tiempoTranscurrido = DateTime.Now - tarea.FechaUltimaRenovacion;

                            // Supongamos que "TiempoEjecucion" está en minutos
                            if (tiempoTranscurrido.TotalMinutes >= tarea.TiempoEjecucion)
                            {
                                // Marcar la tarea como expirada o cambiar su estado según la lógica de negocio.
                                tarea.Estado = false;
                                _logger.LogInformation($"La tarea {tarea.TareaRecurrId} ha expirado.");
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