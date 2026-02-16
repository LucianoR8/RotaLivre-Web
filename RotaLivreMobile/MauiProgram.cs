using Microsoft.Extensions.Logging;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Views;

namespace RotaLivreMobile
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
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // 🔐 Services
            builder.Services.AddSingleton<ApiService>();

            // 🧠 ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();

            // 📱 Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();

            return builder.Build();
        }
    }
}