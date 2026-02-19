using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

public partial class PasseioDetalhePage : ContentPage, IQueryAttributable
{
    private readonly PasseioDetalheViewModel _viewModel;

    public PasseioDetalhePage(PasseioDetalheViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("PasseioId", out var value))
        {
            int id = (int)value;
            await _viewModel.Inicializar(id);
        }
    }
}
