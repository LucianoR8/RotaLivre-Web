using Microsoft.Extensions.Logging;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Views;
using Mapsui;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace RotaLivreMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp(true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<PasseioApiService>();
            builder.Services.AddSingleton<AuthService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<PasseioDetalheViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PasseioDetalhePage>();

            return builder.Build();
        }
    }
}