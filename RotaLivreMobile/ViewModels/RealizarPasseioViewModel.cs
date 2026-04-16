using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Devices.Sensors;

public class RealizarPasseioViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _distanciaTexto;
    public string DistanciaTexto
    {
        get => _distanciaTexto;
        set { _distanciaTexto = value; OnPropertyChanged(); }
    }

    private string _statusArea;
    public string StatusArea
    {
        get => _statusArea;
        set { _statusArea = value; OnPropertyChanged(); }
    }

    private bool estavaDentro = false;

    // Coordenadas do passeio (exemplo)
    private double latPasseio = -23.5505;
    private double lngPasseio = -46.6333;
    private double raio = 100; // metros

    public async Task IniciarMonitoramento()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Permissão",
                "Precisamos da sua localização para o passeio",
                "OK");

            return;
        }

        while (true)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    var distancia = Location.CalculateDistance(
                        location,
                        new Location(latPasseio, lngPasseio),
                        DistanceUnits.Kilometers
                    ) * 1000;

                    DistanciaTexto = $"Distância: {distancia:F0} metros";

                    bool dentro = distancia <= raio;

                    StatusArea = dentro ? "Dentro da área" : "Fora da área";

                    if (dentro && !estavaDentro)
                        await Application.Current.MainPage.DisplayAlert("Geofence", "Você entrou na área", "OK");

                    if (!dentro && estavaDentro)
                        await Application.Current.MainPage.DisplayAlert("Geofence", "Você saiu da área", "OK");

                    estavaDentro = dentro;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO GEO: {ex.Message}");
            }

            await Task.Delay(5000);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}