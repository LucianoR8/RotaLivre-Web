using System.Windows.Input;
using Microsoft.Maui.Storage;
using RotaLivreMobile.Services;
using RotaLivreMobile.Views;

namespace RotaLivreMobile.ViewModels;

public class PerfilViewModel : BaseViewModel
{
    public ICommand LogoutCommand { get; }

    public PerfilViewModel()
    {
        LogoutCommand = new Command(async () => await Logout());
    }

    private async Task Logout()
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Sair",
            "Deseja realmente sair da conta?",
            "Sim",
            "Cancelar"
        );

        if (!confirm)
            return;

        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("usuario_id");
        SecureStorage.Remove("usuario_nome");

        var loginPage = Application.Current
            .Handler
            .MauiContext
            .Services
            .GetService<LoginPage>();

        Application.Current.MainPage = new NavigationPage(loginPage);
    }
}