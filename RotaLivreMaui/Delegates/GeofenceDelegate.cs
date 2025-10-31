using Shiny.Locations;
using Microsoft.Extensions.Logging;

namespace RotaLivreMaui.Delegates;

public class GeofenceDelegate : IGeofenceDelegate
{
    readonly ILogger<GeofenceDelegate> logger;

    public GeofenceDelegate(ILogger<GeofenceDelegate> logger)
    {
        this.logger = logger;
    }

    public Task OnStatusChanged(GeofenceState newState, GeofenceRegion region)
    {
        logger.LogInformation($"Geofence '{region.Identifier}' mudou para {newState}");
        return Task.CompletedTask;
    }
}
