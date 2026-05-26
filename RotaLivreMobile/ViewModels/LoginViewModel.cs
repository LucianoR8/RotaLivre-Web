using System.Windows.Input;
using RotaLivreMobile.Services;
using RotaLivreMobile.Views;

namespace RotaLivreMobile.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    public string Email { get; set; }
    public string Senha { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand IrParaCadastroCommand { get; }
    public ICommand IrParaRecuperacaoCommand { get; }

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoginCommand =
            new Command(async () => await OnLogin());

        IrParaCadastroCommand =
            new Command(OnIrParaCadastro);

        IrParaRecuperacaoCommand =
            new Command(async () =>
            {
                await Shell.Current.GoToAsync(
                    nameof(RecuperarSenhaPage));
            }); 
    }

    private async Task OnLogin()
    {
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;

            bool sucesso =
                await _apiService.Login(Email, Senha);

            if (sucesso)
            {
                await Shell.Current.GoToAsync("//HomePage");

                await Task.Delay(500);

                var app = (App)Application.Current;

                if (!string.IsNullOrEmpty(app.CodigoDeepLink))
                {
                    await Shell.Current.GoToAsync(
                        $"grupoDetalhe?codigo={app.CodigoDeepLink}");

                    app.CodigoDeepLink = null;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro",
                    "Email ou senha inválidos",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Erro",
                ex.Message,
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void OnIrParaCadastro()
    {
        await Shell.Current.GoToAsync(
            nameof(CadastroPage));
    }
}