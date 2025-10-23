using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RotaLivreApp.Services;    
using Shiny;
using Shiny.Hosting;
using Shiny.Locations;
using System;

namespace RotaLivreApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()      
            .UseShiny()              
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.UseGps();        
        builder.Services.UseGeofencing();  
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://127.0.0.1:5001/");
        });

        builder.Services.AddSingleton<GeofenceService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
