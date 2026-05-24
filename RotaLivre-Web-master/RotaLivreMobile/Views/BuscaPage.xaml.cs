using RotaLivreMobile.Models;
using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(Termo), "termo")]
public partial class BuscaPage : ContentPage
{
    private readonly BuscaViewModel _viewModel;

    public BuscaPage(BuscaViewModel vm)
    {
        InitializeComponent();

        BindingContext = _viewModel = vm;
    }

    public string Termo
    {
        set
        {
            _viewModel.BuscarCommand.Execute(Uri.UnescapeDataString(value));
        }
    }

    private async void OnPasseioTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not PasseioDto passeio)
            return;

        await Shell.Current.GoToAsync("detalhe",
            new Dictionary<string, object>
            {
                { "PasseioId", passeio.Id }
            });
    }
}