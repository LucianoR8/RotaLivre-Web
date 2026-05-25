using System;

namespace RotaLivreMobile.Services;

public class LocationService
{
    public async Task<Location?> GetLocationAsync()
    {
        try
        {
            // Verifica se o dispositivo suporta GPS
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                throw new PermissionException("Permissão de localização negada.");

            var request = new GeolocationRequest(
                GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(10));

            return await Geolocation.Default.GetLocationAsync(request);
        }
        catch (FeatureNotSupportedException)
        {
            throw new Exception("GPS não suportado neste dispositivo.");
        }
        catch (FeatureNotEnabledException)
        {
            throw new Exception("GPS está desativado. Ative nas configurações do dispositivo.");
        }
        catch (PermissionException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception)
        {
            throw new Exception("Não foi possível obter a localização. Tente novamente.");
        }
    }
}