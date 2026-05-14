using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(Email), "email")]
public partial class RedefinirSenhaPage : ContentPage
{
    private UsuarioApiService _service;

    public string Email { get; set; }

    public RedefinirSenhaPage()
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

    private async void OnAlterarSenhaClicked(
        object sender,
        EventArgs e)
    {
        var novaSenha =
            novaSenhaEntry.Text?.Trim();

        var confirmarSenha =
            confirmarSenhaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(novaSenha) ||
            string.IsNullOrWhiteSpace(confirmarSenha))
        {
            await DisplayAlert(
                "Erro",
                "Preencha todos os campos",
                "OK");

            return;
        }

        if (novaSenha != confirmarSenha)
        {
            await DisplayAlert(
                "Erro",
                "As senhas não coincidem",
                "OK");

            return;
        }

        var sucesso =
            await _service.RedefinirSenhaAsync(
                Email,
                novaSenha);

        if (!sucesso)
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível alterar a senha",
                "OK");

            return;
        }

        await DisplayAlert(
            "Sucesso",
            "Senha alterada com sucesso",
            "OK");

        await Shell.Current.GoToAsync("//LoginPage");
    }
}