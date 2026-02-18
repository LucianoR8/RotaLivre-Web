using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(Id), "id")]
public partial class PasseioDetalhePage : ContentPage
{
    private readonly PasseioDetalheViewModel _viewModel;

    public string Id
    {
        set
        {
            if (int.TryParse(value, out int id))
                _viewModel.Carregar(id);
        }
    }

    public PasseioDetalhePage(PasseioDetalheViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}