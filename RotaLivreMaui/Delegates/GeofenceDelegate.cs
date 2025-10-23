using Shiny.Locations;
using Microsoft.Extensions.Logging;

namespace RotaLivreMaui.Delegates;

public class GeofenceDelegate : IGeofenceDelegate
{
    readonly ILogger<GeofenceDelegate> _logger;
    public GeofenceDelegate(ILogger<GeofenceDelegate> logger) => _logger = logger;

    public Task OnStatusChanged(GeofenceState newState, GeofenceRegion region)
    {
        _logger.LogInformation($"Geofence {region.Identifier} mudou para {newState}");
        return Task.CompletedTask;
    }
}
