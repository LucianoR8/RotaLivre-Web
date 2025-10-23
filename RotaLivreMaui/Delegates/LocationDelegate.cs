using Shiny.Locations;
using Microsoft.Extensions.Logging;

namespace RotaLivreMaui.Delegates;

public class LocationDelegate : ILocationDelegate
{
    readonly ILogger<LocationDelegate> _logger;
    public LocationDelegate(ILogger<LocationDelegate> logger) => _logger = logger;

    public Task OnReading(LocationReading reading)
    {
        _logger.LogInformation($"Nova localização: {reading.Position.Latitude}, {reading.Position.Longitude}");
        return Task.CompletedTask;
    }
}
