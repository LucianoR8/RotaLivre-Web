using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Microsoft.Maui.Controls;
using NetTopologySuite.Geometries;
using RotaLivreMobile.Services;
using SkiaSharp;
using MapsuiBrush = Mapsui.Styles.Brush;
using MapsuiColor = Mapsui.Styles.Color;
using MapsuiPen = Mapsui.Styles.Pen;
using MauiColor = Microsoft.Maui.Graphics.Color;
using MauiFeature = Mapsui.Nts.GeometryFeature;
using NtsPoint = NetTopologySuite.Geometries.Point;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;

namespace RotaLivreMobile.Views;

// Estilo customizado com SkiaSharp para transparência real
public class GeofenceSkiaStyle : IStyle
{
    public bool Dentro { get; set; }
    public double MinVisible { get; set; } = 0;
    public double MaxVisible { get; set; } = double.MaxValue;
    public bool Enabled { get; set; } = true;
    public float Opacity { get; set; } = 1f;
}

// Renderer do estilo customizado
public class GeofenceSkiaStyleRenderer : ISkiaStyleRenderer
{
    public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer,
        IFeature feature, IStyle style, RenderService renderService, long iteration)
    {
        Console.WriteLine("[SKIA] Draw chamado!");

        if (style is not GeofenceSkiaStyle geofenceStyle) return false;
        if (feature is not MauiFeature geomFeature) return false;
        if (geomFeature.Geometry is not NtsPolygon polygon) return false;

        var coords = polygon.ExteriorRing.Coordinates;
        var path = new SKPath();
        bool first = true;

        foreach (var coord in coords)
        {
            var screen = viewport.WorldToScreen(coord.X, coord.Y);
            if (first)
            {
                path.MoveTo((float)screen.X, (float)screen.Y);
                first = false;
            }
            else
            {
                path.LineTo((float)screen.X, (float)screen.Y);
            }
        }
        path.Close();

        using var fillPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = geofenceStyle.Dentro
                ? new SKColor(102, 187, 106, 40)
                : new SKColor(229, 115, 115, 40)
        };

        using var strokePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 3,
            Color = geofenceStyle.Dentro
                ? new SKColor(76, 175, 80, 255)
                : new SKColor(229, 57, 53, 255)
        };

        canvas.DrawPath(path, fillPaint);
        canvas.DrawPath(path, strokePaint);

        return true;
    }
}

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

        // Registra o renderer customizado
        Mapsui.Rendering.Skia.MapRenderer.RegisterStyleRenderer(
            typeof(GeofenceSkiaStyle),
            new GeofenceSkiaStyleRenderer());
    }

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
                    $"{(_geofencing == null ? "NULL" : $"Lat={_geofencing.Latitude}, Lon={_geofencing.Longitude}, Raio={_geofencing.RaioMetros}")}");

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
            LoadingOverlay.IsVisible = true;

            var location = await _locationService.GetLocationAsync();
            if (location == null)
            {
                Console.WriteLine("[GEOFENCING] localização do usuário é NULL");
                return;
            }

            var userSpherical = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
            var userCenter = new MPoint(userSpherical.x, userSpherical.y);

            mapControl.Map.Navigator.CenterOn(userCenter);
            mapControl.Map.Navigator.ZoomTo(1000);

            RemoverCamada("UserPin");
            RemoverCamada("Geofencing");

            if (_geofencing != null)
            {
                bool dentroDoRaio = VerificarGeofencing(
                    location.Latitude, location.Longitude,
                    _geofencing.Latitude, _geofencing.Longitude,
                    _geofencing.RaioMetros);

                DesenharGeofencing(
                    _geofencing.Latitude, _geofencing.Longitude,
                    _geofencing.RaioMetros, dentroDoRaio);

                StatusLabel.Text = dentroDoRaio
                    ? "Você está dentro da área do passeio"
                    : "Você está fora da área do passeio";

                StatusLabel.BackgroundColor = dentroDoRaio
                    ? MauiColor.FromArgb("#2E7D32")
                    : MauiColor.FromArgb("#C62828");
            }

            AdicionarPinUsuario(userCenter);
            mapControl.Map.RefreshGraphics();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private bool VerificarGeofencing(double userLat, double userLon,
        double centerLat, double centerLon, int raioMetros)
    {
        var distanciaKm = Microsoft.Maui.Devices.Sensors.Location.CalculateDistance(
            userLat, userLon, centerLat, centerLon, DistanceUnits.Kilometers);

        double distanciaMetros = distanciaKm * 1000;
        Console.WriteLine($"[GEOFENCING] distância: {distanciaMetros} metros");

        return distanciaMetros <= raioMetros;
    }

    private void DesenharGeofencing(double lat, double lon, int raioMetros, bool dentro)
    {
        // 1. Garante a projeção correta de EPSG:4326 (WGS84) para EPSG:3857 (Spherical Mercator)
        var spherical = SphericalMercator.FromLonLat(lon, lat);

        var coordinates = new List<Coordinate>();
        for (int i = 0; i <= 64; i++)
        {
            double angle = i * Math.PI * 2 / 64;
            coordinates.Add(new Coordinate(
                spherical.x + raioMetros * Math.Cos(angle),
                spherical.y + raioMetros * Math.Sin(angle)));
        }

        // Define as cores utilizando o padrão MapsuiColor (Alpha, R, G, B)
        // O valor 50 de Alpha garante transparência de ~20% para ver as ruas por trás
        var corPreenchimento = dentro
            ? MapsuiColor.FromArgb(50, 102, 187, 106)
            : MapsuiColor.FromArgb(50, 229, 115, 115);

        var corBorda = dentro
            ? MapsuiColor.FromArgb(255, 76, 175, 80)
            : MapsuiColor.FromArgb(255, 229, 57, 53);

        // 2. IMPORTANTE: Criamos o estilo diretamente na Camada para evitar conflitos de herança
        var estiloCamada = new VectorStyle
        {
            Fill = new MapsuiBrush(corPreenchimento),
            Outline = new MapsuiPen
            {
                Color = corBorda,
                Width = 3
            }
        };

        var feature = new MauiFeature
        {
            Geometry = new NtsPolygon(new LinearRing(coordinates.ToArray()))
        };
        // Deixamos a feature sem estilos próprios para ela herdar diretamente da camada

        // 3. Inicialização explícita da camada com o estilo transparente atribuído
        var layer = new MemoryLayer
        {
            Name = "Geofencing",
            Features = new[] { feature },
            Style = estiloCamada // Vincula o estilo transparente diretamente aqui
        };

        // Adiciona a camada ao mapa
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
                            Fill = new MapsuiBrush(MapsuiColor.FromString("#2E86AB")),
                            Outline = new MapsuiPen(MapsuiColor.White, 3)
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