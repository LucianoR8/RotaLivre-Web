using Shiny.Locations;
using Microsoft.Extensions.Logging;

namespace RotaLivreMaui.Delegates
{
    public class LocationDelegate : IGpsDelegate
    {
        readonly ILogger<LocationDelegate> _logger;
        public LocationDelegate(ILogger<LocationDelegate> logger) => _logger = logger;

        public Task OnReading(GpsReading reading)
        {
            _logger.LogInformation($"Nova leitura GPS: {reading.Position.Latitude}, {reading.Position.Longitude}");
            return Task.CompletedTask;
        }
    }
}
