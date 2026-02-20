using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(CategoriaId), "categoriaId")]
[QueryProperty(nameof(CategoriaNome), "categoriaNome")]
public partial class CategoriaPage : ContentPage
{
    private CategoriaViewModel ViewModel => BindingContext as CategoriaViewModel;

    public string CategoriaId
    {
        set
        {
            ViewModel.CategoriaId = int.Parse(value);
        }
    }

    public string CategoriaNome
    {
        set
        {
            Title = Uri.UnescapeDataString(value);
        }
    }

    public CategoriaPage()
    {
        InitializeComponent();
        BindingContext = new CategoriaViewModel();
    }

    private async void OnPasseioSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var passeio = e.CurrentSelection[0] as PasseioDto;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync($"DetalhePage?passeioId={passeio.Id}");
    }
}