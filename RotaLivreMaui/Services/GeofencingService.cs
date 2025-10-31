/* using Microsoft.Extensions.Logging;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RotaLivreMaui.Services
{
    public class GeofencingService
    {
        private readonly IGeofenceManager _geofenceManager;
        private readonly ILogger<GeofencingService> _logger;
        private readonly INotificationManager _notificationManager;
        private readonly ApiService _apiService;

        public GeofencingService(
            IGeofenceManager geofenceManager,
            ILogger<GeofencingService> logger,
            INotificationManager notificationManager,
            ApiService apiService)
        {
            _geofenceManager = geofenceManager;
            _logger = logger;
            _notificationManager = notificationManager;
            _apiService = apiService;
        }

        public async Task InitializeGeofencesAsync()
        {
            var passeios = await _apiService.GetPasseiosAsync();

            foreach (var passeio in passeios.Where(p => p.latitude != 0 && p.longitude != 0))
            {
                var fenceId = $"passeio_{passeio.id_passeio}";

                // Remove anterior caso exista
                await _geofenceManager.StopMonitoring(fenceId);

                var region = new GeofenceRegion(
                    fenceId,
                    new Position(passeio.latitude, passeio.longitude),
                    Distance.FromMeters(150),
                    true,   // notifyOnEntry
                    false   // notifyOnExit
                );

                await _geofenceManager.StartMonitoring(region);
                _logger.LogInformation($"✅ Geofence criada para {passeio.nome_passeio} ({passeio.latitude}, {passeio.longitude})");
            }
        }

        // Em Shiny 3.3.4 o tipo correto é GeofenceArgs
        public async Task OnGeofenceEvent(GeofenceArgs e)
        {
            try
            {
                if (e.Transition == GeofenceState.Entered)
                {
                    await _notificationManager.Send(new Notification
                    {
                        Title = "Você chegou em um passeio!",
                        Message = $"Você entrou em {e.Region.Identifier}"
                    });

                    _logger.LogInformation($"Entrou em {e.Region.Identifier}");
                }
                else if (e.Transition == GeofenceState.Exited)
                {
                    await _notificationManager.Send(new Notification
                    {
                        Title = "Saída de área detectada",
                        Message = $"Você saiu de {e.Region.Identifier}"
                    });

                    _logger.LogInformation($"Saiu de {e.Region.Identifier}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento de geofence");
            }
        }
    }
}
*/