using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(Email), "email")]
[QueryProperty(nameof(Pergunta), "pergunta")]
public partial class ConfirmarRespostaPage : ContentPage
{
    private UsuarioApiService _service;

    public string Email { get; set; }

    public string Pergunta
    {
        get => perguntaLabel.Text;
        set => perguntaLabel.Text = Uri.UnescapeDataString(value);
    }

    public ConfirmarRespostaPage()
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

    private async void OnConfirmarClicked(
        object sender,
        EventArgs e)
    {
        var resposta =
            respostaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(resposta))
        {
            await DisplayAlert(
                "Erro",
                "Digite a resposta",
                "OK");

            return;
        }

        var sucesso =
            await _service.VerificarRespostaAsync(
                Email,
                resposta);

        if (!sucesso)
        {
            await DisplayAlert(
                "Erro",
                "Resposta incorreta",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(RedefinirSenhaPage)}" +
            $"?email={Email}");
    }
}