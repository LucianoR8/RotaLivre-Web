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

    public MapaGrupoPage(LocationService locationService, EnderecoService enderecoService)
    {
        InitializeComponent();
        _locationService = locationService;
        _enderecoService = enderecoService;
        mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }

    // Recebe o idPasseio navegando para a página
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("idPasseio", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                Console.WriteLine($"[GEOFENCING] idPasseio recebido: {id}");
                _geofencing = await _enderecoService.GetGeofencingAsync(id);
                Console.WriteLine($"[GEOFENCING] resultado: {(_geofencing == null ? "NULL" : $"Lat={_geofencing.Latitude}, Lon={_geofencing.Longitude}, Raio={_geofencing.RaioMetros}")}");
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

        {
            if (query.TryGetValue("idPasseio", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                Console.WriteLine($"[GEOFENCING] idPasseio recebido: {id}");
                _geofencing = await _enderecoService.GetGeofencingAsync(id);
                Console.WriteLine($"[GEOFENCING] resultado: {(_geofencing == null ? "NULL" : $"Lat={_geofencing.Latitude}, Lon={_geofencing.Longitude}, Raio={_geofencing.RaioMetros}")}");
            }
            else
            {
                Console.WriteLine("[GEOFENCING] idPasseio NÃO recebido na query!");
            }
        }
    }    
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AtualizarMapaAsync();
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        await AtualizarMapaAsync();
    }

    private async Task AtualizarMapaAsync()
    {
        try
        {
            // Diagnóstico temporário
            await DisplayAlert("Debug",
                $"Geofencing: {(_geofencing == null ? "NULL" : $"Lat={_geofencing.Latitude}, Lon={_geofencing.Longitude}, Raio={_geofencing.RaioMetros}")}",
                "OK");

            var location = await _locationService.GetLocationAsync();
            if (location == null)
            {
                Console.WriteLine("[GEOFENCING] localização do usuário é NULL");
                return;
            }

            Console.WriteLine($"[GEOFENCING] usuário em Lat={location.Latitude}, Lon={location.Longitude}");
            Console.WriteLine($"[GEOFENCING] _geofencing é {(_geofencing == null ? "NULL" : "OK")}");


            // Posição do usuário
            var userSpherical = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
            var userCenter = new MPoint(userSpherical.x, userSpherical.y);

            // Centraliza e zoom
            mapControl.Map.Navigator.CenterOn(userCenter);
            mapControl.Map.Navigator.ZoomTo(50);

            // Remove camadas antigas
            RemoverCamada("UserPin");
            RemoverCamada("Geofencing");

            // Pin do usuário
            AdicionarPinUsuario(userCenter);

            // Geofencing
            if (_geofencing?.Latitude != null && _geofencing?.Longitude != null)
            {
                bool dentroDoRaio = VerificarGeofencing(
                    location.Latitude, location.Longitude,
                    _geofencing.Latitude, _geofencing.Longitude,
                    _geofencing.RaioMetros);

                DesenharGeofencing(_geofencing.Latitude, _geofencing.Longitude,
                    _geofencing.RaioMetros, dentroDoRaio);

                // Banner de status
                StatusLabel.Text = dentroDoRaio ? "✅ Você está dentro da área do passeio"
                                                : "⚠️ Você está fora da área do passeio";
                StatusLabel.BackgroundColor = dentroDoRaio ? MauiColor.FromArgb("#2E7D32")
                                                           : MauiColor.FromArgb("#C62828");
            }

            mapControl.Map.RefreshGraphics();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private bool VerificarGeofencing(double userLat, double userLon,
        double centerLat, double centerLon, int raioMetros)
    {
        var userPoint = new NtsPoint(userLon, userLat);
        var centerPoint = new NtsPoint(centerLon, centerLat);

        // Distância em graus aproximada (1 grau ≈ 111km)
        double distanciaMetros = userPoint.Distance(centerPoint) * 111000;

        return distanciaMetros <= raioMetros;
    }

    private void DesenharGeofencing(double lat, double lon, int raioMetros, bool dentro)
    {
        var spherical = SphericalMercator.FromLonLat(lon, lat);
        var center = new MPoint(spherical.x, spherical.y);

        // Gera círculo com 64 pontos
        var pontos = new List<MPoint>();
        for (int i = 0; i <= 64; i++)
        {
            double angulo = 2 * Math.PI * i / 64;
            pontos.Add(new MPoint(
                center.X + raioMetros * Math.Cos(angulo),
                center.Y + raioMetros * Math.Sin(angulo)));
        }

        var corFundo = dentro
            ? new MapsuiColor(46, 125, 50, 80)    // verde transparente
            : new MapsuiColor(198, 40, 40, 80);   // vermelho transparente

        var corBorda = dentro
            ? Mapsui.Styles.Color.FromString("#2E7D32")
            : Mapsui.Styles.Color.FromString("#C62828");

        var feature = new MauiFeature
        {
            Geometry = new NetTopologySuite.Geometries.Polygon(
                new LinearRing(pontos.Select(p =>
                    new Coordinate(p.X, p.Y)).ToArray()))
        };

        feature.Styles.Add(new Mapsui.Styles.VectorStyle
        {
            Fill = new MapsuiBrush(corFundo),
            Outline = new MapsuiPen(corBorda, 2)
        });

        var layer = new MemoryLayer("Geofencing") { Features = new[] { feature } };
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
                            SymbolScale = 0.7,
                            Fill = new MapsuiBrush(MapsuiColor.FromString("#2E86AB")),
                            Outline = new MapsuiPen(Mapsui.Styles.Color.White, 2)
                        }
                    }
                }
            }
        };
        mapControl.Map.Layers.Add(layer);
    }

    private void RemoverCamada(string nome)
    {
        var layer = mapControl.Map.Layers.FindLayer(nome).FirstOrDefault();
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