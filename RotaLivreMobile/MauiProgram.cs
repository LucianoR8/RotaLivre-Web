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
                .UseSkiaSharp()
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

            builder.Services.AddHttpClient<UsuarioApiService>(client =>
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
            builder.Services.AddTransient<GrupoViewModel>();
            builder.Services.AddTransient<CategoriaViewModel>();
            builder.Services.AddTransient<CadastroViewModel>();
            builder.Services.AddTransient<MeusPasseiosViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<EditarPerfilViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PasseioDetalhePage>();
            builder.Services.AddTransient<GrupoPage>();
            builder.Services.AddTransient<CategoriaPage>();
            builder.Services.AddTransient<CadastroPage>();
            builder.Services.AddTransient<MeusPasseiosPage>();
            builder.Services.AddTransient<PerfilPage>();
            builder.Services.AddTransient<EditarPerfilPage>();

            return builder.Build();
        }
    }
}