using Microsoft.Extensions.Logging;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RotaLivreMaui.Services
{
    public class BackgroundGeofencingService
    {
        readonly IGpsManager _gpsManager;
        readonly INotificationManager _notificationManager;
        readonly ILogger<BackgroundGeofencingService> _logger;
        readonly ApiService _api;

        public BackgroundGeofencingService(
            IGpsManager gpsManager,
            INotificationManager notificationManager,
            ILogger<BackgroundGeofencingService> logger,
            ApiService api)
        {
            _gpsManager = gpsManager;
            _notificationManager = notificationManager;
            _logger = logger;
            _api = api;
        }

        public async Task StartAsync()
        {
            // ✅ Nova forma de criar o request no Shiny 3.3.4
            var request = new GpsRequest
            {
                Accuracy = GpsAccuracy.High,     // “Best” foi removido
                Interval = TimeSpan.FromSeconds(5)
            };

            var access = await _gpsManager.RequestAccess(request);
            if (access != AccessState.Available)
            {
                _logger.LogWarning($"GPS não disponível: {access}");
                return;
            }

            // ✅ Nova assinatura — não precisa de parâmetro extra
            _gpsManager
                .WhenReading()
                .Subscribe(async reading => await HandleReading(reading));
        }

        private async Task HandleReading(GpsReading reading)
        {
            try
            {
                var passeios = await _api.GetPasseiosAsync();
                foreach (var p in passeios.Where(x => x.latitude != 0 && x.longitude != 0))
                {
                    double km = HaversineDistance(
                        reading.Position.Latitude,
                        reading.Position.Longitude,
                        p.latitude,
                        p.longitude
                    );

                    // km -> usamos 0.15 = 150m
                    if (km <= 0.15)
                    {
                        // ✅ Ajuste na notificação (Shiny 3.3.4 mudou a API)
                        await _notificationManager.Send(
                            "Você chegou em um passeio!",
                            $"{p.nome_passeio} — a {Math.Round(km * 1000)} m"
                        );

                        _logger.LogInformation($"Entrou no raio de {p.nome_passeio} ({km} km)");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar leitura GPS");
            }
        }

        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // km
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;
    }
}
