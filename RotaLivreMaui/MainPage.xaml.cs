using Microsoft.Extensions.DependencyInjection;
using RotaLivreMaui.Services;



namespace RotaLivreMaui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var service = App.Current?.Handler?.MauiContext?.Services?.GetService<BackgroundGeofencingService>();
        if (service != null)
            await service.StartAsync();
    }


}
