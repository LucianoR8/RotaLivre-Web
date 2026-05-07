using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(CategoriaId), "categoriaId")]
[QueryProperty(nameof(CategoriaNome), "categoriaNome")]
public partial class CategoriaPage : ContentPage
{
    private CategoriaViewModel _viewModel;

    public CategoriaPage(CategoriaViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public string CategoriaId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                _viewModel.CategoriaId = id;
            }
        }
    }

    public string CategoriaNome
    {
        set
        {
            Title = Uri.UnescapeDataString(value);
        }
    }

    private async void OnPasseioClicado(object sender, TappedEventArgs e)
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