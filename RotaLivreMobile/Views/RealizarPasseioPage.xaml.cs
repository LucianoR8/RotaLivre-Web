using Mapsui;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Microsoft.Maui.ApplicationModel;
using RotaLivreMobile.Models;
using RotaLivreMobile.ViewModels;



namespace RotaLivreMobile.Views;
public partial class RealizarPasseioPage : ContentPage
{
    private RealizarPasseioViewModel _viewModel;

    public RealizarPasseioPage()
    {
        InitializeComponent();

        _viewModel = new RealizarPasseioViewModel();
        BindingContext = _viewModel;

        map.Map = new Mapsui.Map();
        map.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.IniciarMonitoramento();
    }
}