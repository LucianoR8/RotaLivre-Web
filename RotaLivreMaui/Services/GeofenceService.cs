using Shiny;
using Shiny.Locations;

namespace RotaLivreMaui.Services;

public class GeofenceService
{
    readonly IGeofenceManager _geofenceManager;

    public GeofenceService(IGeofenceManager geofenceManager)
    {
        _geofenceManager = geofenceManager;
    }

    public async Task RegistrarGeofence(double latitude, double longitude, string nome)
    {
        var pos = new Position(latitude, longitude);
        var radius = Distance.FromMeters(200);

        var region = new GeofenceRegion(nome, pos, radius)
        {
            NotifyOnEntry = true,
            NotifyOnExit = false
        };

        await _geofenceManager.StartMonitoring(region);
    }
}
