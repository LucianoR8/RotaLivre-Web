using Microsoft.Maui.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;
using RotaLivreMobile.Services;

using MauiColor = Microsoft.Maui.Graphics.Color;
using MapsuiColor = Mapsui.Styles.Color;
using MapsuiBrush = Mapsui.Styles.Brush;
using MapsuiPen = Mapsui.Styles.Pen;
using NtsPoint = NetTopologySuite.Geometries.Point;
using MauiFeature = Mapsui.Nts.GeometryFeature;

namespace RotaLivreMobile.Views;

public partial class MapaGrupoPage : ContentPage, IQueryAttributable
{
    private readonly LocationService _locationService;
    private readonly EnderecoService _enderecoService;

    private GeofencingInfo? _geofencing;

    public MapaGrupoPage(
        LocationService locationService,
        EnderecoService enderecoService)
    {
        InitializeComponent();

        _locationService = locationService;
        _enderecoService = enderecoService;

        mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }

    // Recebe o idPasseio pela navegação
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("idPasseio", out var value)
                && int.TryParse(value?.ToString(), out int id))
            {
                Console.WriteLine($"[GEOFENCING] idPasseio recebido: {id}");

                _geofencing = await _enderecoService.GetGeofencingAsync(id);

                Console.WriteLine(
                    $"[GEOFENCING] resultado: " +
                    $"{(_geofencing == null
                        ? "NULL"
                        : $"Lat={_geofencing.Latitude}, Lon={_geofencing.Longitude}, Raio={_geofencing.RaioMetros}")}");

                await AtualizarMapaAsync();
            }
            else
            {
                Console.WriteLine("[GEOFENCING] idPasseio NÃO recebido na query!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GEOFENCING] ERRO: {ex.Message}");
        }
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        await AtualizarMapaAsync();
    }

    private async Task AtualizarMapaAsync()
    {
        try
        {
            var location = await _locationService.GetLocationAsync();

            if (location == null)
            {
                Console.WriteLine("[GEOFENCING] localização do usuário é NULL");
                return;
            }

            Console.WriteLine(
                $"[GEOFENCING] usuário em Lat={location.Latitude}, Lon={location.Longitude}");

            // Converte posição do usuário
            var userSpherical = SphericalMercator.FromLonLat(
                location.Longitude,
                location.Latitude);

            var userCenter = new MPoint(
                userSpherical.x,
                userSpherical.y);

            // Centraliza mapa
            mapControl.Map.Navigator.CenterOn(userCenter);

            // Zoom mais confortável
            mapControl.Map.Navigator.ZoomTo(1000);

            // Remove camadas antigas
            RemoverCamada("UserPin");
            RemoverCamada("Geofencing");

            // Primeiro desenha o geofencing
            if (_geofencing != null)
            {
                bool dentroDoRaio = VerificarGeofencing(
                    location.Latitude,
                    location.Longitude,
                    _geofencing.Latitude,
                    _geofencing.Longitude,
                    _geofencing.RaioMetros);

                DesenharGeofencing(
                    _geofencing.Latitude,
                    _geofencing.Longitude,
                    _geofencing.RaioMetros,
                    dentroDoRaio);

                // Status visual
                StatusLabel.Text = dentroDoRaio
                    ? "Você está dentro da área do passeio"
                    : "Você está fora da área do passeio";

                StatusLabel.BackgroundColor = dentroDoRaio
                    ? MauiColor.FromArgb("#2E7D32")
                    : MauiColor.FromArgb("#C62828");
            }

            // Depois desenha o usuário POR CIMA
            AdicionarPinUsuario(userCenter);

            mapControl.Map.RefreshGraphics();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private bool VerificarGeofencing(
        double userLat,
        double userLon,
        double centerLat,
        double centerLon,
        int raioMetros)
    {
        var distanciaKm =
            Microsoft.Maui.Devices.Sensors.Location.CalculateDistance(
                userLat,
                userLon,
                centerLat,
                centerLon,
                DistanceUnits.Kilometers);

        double distanciaMetros = distanciaKm * 1000;

        Console.WriteLine(
            $"[GEOFENCING] distância: {distanciaMetros} metros");

        return distanciaMetros <= raioMetros;
    }

    private void DesenharGeofencing(
    double lat,
    double lon,
    int raioMetros,
    bool dentro)
    {
        // Centro convertido para coordenadas do mapa
        var spherical = SphericalMercator.FromLonLat(lon, lat);

        // Cria pontos do círculo
        var coordinates = new List<Coordinate>();

        for (int i = 0; i <= 64; i++)
        {
            double angle = i * Math.PI * 2 / 64;

            double dx = raioMetros * Math.Cos(angle);
            double dy = raioMetros * Math.Sin(angle);

            coordinates.Add(new Coordinate(
                spherical.x + dx,
                spherical.y + dy));
        }

        // Cria apenas uma linha circular
        var lineString = new LineString(coordinates.ToArray());

        var feature = new MauiFeature
        {
            Geometry = lineString
        };

        var corBorda = dentro
            ? MapsuiColor.FromString("#66BB6A")
            : MapsuiColor.FromString("#E57373");

        feature.Styles.Add(new VectorStyle
        {
            Line = new MapsuiPen(corBorda, 4)
        });

        var layer = new MemoryLayer("Geofencing")
        {
            Features = new[] { feature }
        };

        mapControl.Map.Layers.Add(layer);
    }

    private void AdicionarPinUsuario(MPoint center)
    {
        var layer = new MemoryLayer("UserPin")
        {
            Features = new[]
            {
                new PointFeature(center)
                {
                    Styles = new[]
                    {
                        new SymbolStyle
                        {
                            SymbolScale = 0.9,

                            Fill = new MapsuiBrush(
                                MapsuiColor.FromString("#2E86AB")),

                            Outline = new MapsuiPen(
                                MapsuiColor.White,
                                3)
                        }
                    }
                }
            }
        };

        mapControl.Map.Layers.Add(layer);
    }

    private void RemoverCamada(string nome)
    {
        var layer = mapControl.Map.Layers
            .FindLayer(nome)
            .FirstOrDefault();

        if (layer != null)
            mapControl.Map.Layers.Remove(layer);
    }

    private void OnZoomInClicked(object sender, EventArgs e)
    {
        mapControl.Map.Navigator.ZoomIn();
        mapControl.Map.RefreshGraphics();
    }

    private void OnZoomOutClicked(object sender, EventArgs e)
    {
        mapControl.Map.Navigator.ZoomOut();
        mapControl.Map.RefreshGraphics();
    }
}