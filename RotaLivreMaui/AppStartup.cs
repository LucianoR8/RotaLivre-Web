using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Hosting;
using Shiny.Locations;
using Shiny.Notifications;
using RotaLivreMaui.Delegates;
using RotaLivreMaui.Services;

namespace RotaLivreMaui
{
    public class AppStartup : FrameworkStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // GPS e Geofencing
            services.UseGps<LocationDelegate>();
            services.UseGeofencing<GeofenceDelegate>();

            // Notificações
            services.UseNotifications();

            // Serviços da aplicação
            services.AddSingleton<ApiService>();
            services.AddSingleton<BackgroundGeofencingService>();
        }
    }
}
