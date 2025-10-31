using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using RotaLivreMaui.Services;

namespace RotaLivreMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseShiny<AppStartup>(); // ✅ método correto

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<BackgroundGeofencingService>();

            return builder.Build();
        }
    }
}
