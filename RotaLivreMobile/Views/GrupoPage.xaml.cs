using RotaLivreMobile.ViewModels;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(NomePasseio), "nomePasseio")]
[QueryProperty(nameof(IdPasseio), "idPasseio")]
[QueryProperty(nameof(Codigo), "codigo")]
public partial class GrupoPage : ContentPage
{
    private readonly GrupoViewModel _viewModel;

    public GrupoPage(GrupoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Title = _viewModel.NomePasseio ?? "Grupo";

        if (BindingContext is GrupoViewModel vm)
        {
            await vm.CarregarGrupoSalvo();
        }
    }

    private async void OnCopiarCodigoClicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(_viewModel.LinkGrupo);

        await DisplayAlert("Copiado", "Link do grupo copiado!", "OK");
    }

    private async void OnCompartilharGrupoClicked(object sender, EventArgs e)
    {
        if (BindingContext is GrupoViewModel vm && !string.IsNullOrEmpty(vm.CodigoGrupo))
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = vm.LinkGrupo,
                Title = "Entrar no grupo de passeio"
            });
        }
    }

    private async void OnSairGrupoClicked(object sender, EventArgs e)
    {
        if (BindingContext is GrupoViewModel vm)
        {
            await vm.SairGrupo();
        }
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

    public string Codigo
    {
        set
        {
            Console.WriteLine($"Chegou com codigo: {value}");

            if (!string.IsNullOrEmpty(value))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    _viewModel.CodigoDigitado = value;
                    await _viewModel.EntrarGrupoDireto();
                });
            }
        }
    }

}