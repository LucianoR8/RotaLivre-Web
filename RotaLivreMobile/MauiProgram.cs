using Microsoft.Extensions.Logging;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Views;
using Mapsui;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;

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

            builder.Services.AddHttpClient<BaseApiService>(client =>
            {
                client.BaseAddress = new Uri("https://rotalivre-web.onrender.com/api/");
                client.DefaultRequestVersion = HttpVersion.Version11;
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
            });

            builder.Services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://rotalivre-web.onrender.com/api/");
                client.DefaultRequestVersion = HttpVersion.Version11;
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
            }); ;

            builder.Services.AddHttpClient<PasseioApiService>(client =>
            {
                client.BaseAddress = new Uri("https://rotalivre-web.onrender.com/api/");
                client.DefaultRequestVersion = HttpVersion.Version11;
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
            });

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<GrupoSignalRService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<PasseioDetalheViewModel>();
            builder.Services.AddSingleton<GrupoViewModel>();
            builder.Services.AddTransient<CategoriaViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PasseioDetalhePage>();
            builder.Services.AddTransient<GrupoPage>();
            builder.Services.AddTransient<CategoriaPage>();


            return builder.Build();
        }
    }
}