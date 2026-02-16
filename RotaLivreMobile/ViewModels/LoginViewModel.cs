using System.Windows.Input;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    public string Email { get; set; }
    public string Senha { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand IrParaCadastroCommand { get; }

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoginCommand = new Command(async () => await OnLogin());
        IrParaCadastroCommand = new Command(OnIrParaCadastro);
    }

    private async Task OnLogin()
    {
        bool sucesso = await _apiService.Login(Email, Senha);

        if (sucesso)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Erro", "Email ou senha inválidos", "OK");
        }
    }

    private async void OnIrParaCadastro()
    {
        await Shell.Current.GoToAsync("CadastroPage");
    }
}