using Mapsui.Layers;
using Microsoft.Maui.Controls;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

public partial class MapaGrupoPage : ContentPage
{
    private readonly LocationService _locationService;

    public MapaGrupoPage(LocationService locationService)
    {
        InitializeComponent();

        _locationService = locationService;

        // Adiciona camada de tiles (OpenStreetMap)
        mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CentralizarNaLocalizacaoAsync();
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        await CentralizarNaLocalizacaoAsync();
    }

    private async Task CentralizarNaLocalizacaoAsync()
    {
        try
        {
            var location = await _locationService.GetLocationAsync();

            if (location != null)
            {
                var spherical = SphericalMercator.FromLonLat(
                    location.Longitude,
                    location.Latitude);

                var center = new MPoint(spherical.x, spherical.y);

                mapControl.Map.Navigator.CenterOn(center);
                mapControl.Map.Navigator.ZoomTo(5);

                // Remove pin anterior se existir
                var oldLayer = mapControl.Map.Layers.FindLayer("UserPin").FirstOrDefault();
                if (oldLayer != null)
                    mapControl.Map.Layers.Remove(oldLayer);

                // Cria o pin com ícone de localização
                var pinLayer = new Mapsui.Layers.MemoryLayer("UserPin")
                {
                    Features = new[]
                    {
                    new PointFeature(center)
                    {
                        Styles = new[]
                        {
                            new Mapsui.Styles.SymbolStyle
                            {
                                SymbolScale = 0.7,
                                Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.FromString("#2E86AB")),
                                Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.White, 2)
                            }
                        }
                    }
                }

                };

                mapControl.Map.Layers.Add(pinLayer);
                mapControl.Map.RefreshGraphics();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Localização", ex.Message, "OK");
        }
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