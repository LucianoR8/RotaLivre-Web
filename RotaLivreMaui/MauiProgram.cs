using Microsoft.Extensions.Logging;
using Shiny;

namespace RotaLivreMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseShiny()
            .UseShinyHosting()
            .UseShinyLocations(cfg =>
            {
                cfg.UseGps();
                cfg.UseGeofencing();
            });

        builder.Logging.AddDebug();

        return builder.Build();
    }
}
