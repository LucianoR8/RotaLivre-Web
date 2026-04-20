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

    private async void OnPasseioSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var passeio = e.CurrentSelection[0] as PasseioDto;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"detalhe?passeioId={passeio.Id}");
    }
}