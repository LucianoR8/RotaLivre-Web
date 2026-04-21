using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(NomePasseio), "nomePasseio")]
[QueryProperty(nameof(IdPasseio), "idPasseio")]
public partial class GrupoPage : ContentPage
{
    private readonly GrupoViewModel _viewModel;

    public GrupoPage(GrupoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public string NomePasseio
    {
        set
        {
            _viewModel.NomePasseio = Uri.UnescapeDataString(value);
        }
    }

    public string IdPasseio
    {
        set
        {
            if (int.TryParse(value, out int id))
                _viewModel.IdPasseio = id;
        }
    }
}