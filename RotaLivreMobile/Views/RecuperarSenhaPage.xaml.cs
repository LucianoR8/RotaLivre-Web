using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

public partial class RecuperarSenhaPage : ContentPage
{
    private UsuarioApiService _service;

    public RecuperarSenhaPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _service ??=
            Handler?.MauiContext?.Services
            .GetService<UsuarioApiService>();
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
        try
        {
            var email = emailEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlert(
                    "Erro",
                    "Digite um e-mail",
                    "OK");

                return;
            }

            var pergunta =
                await _service.BuscarPerguntaSeguranca(email);

            if (string.IsNullOrWhiteSpace(pergunta))
            {
                await DisplayAlert(
                    "Erro",
                    "E-mail não encontrado",
                    "OK");

                return;
            }

            await Shell.Current.GoToAsync(
                $"{nameof(ConfirmarRespostaPage)}" +
                $"?email={email}" +
                $"&pergunta={Uri.EscapeDataString(pergunta)}");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro",
                ex.ToString(),
                "OK");
        }
    }
}