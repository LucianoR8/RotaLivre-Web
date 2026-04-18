using Microsoft.Maui.Storage;
using RotaLivreMobile.Views;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;

namespace RotaLivreMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage();
            VerificarLogin();
        }

        private async void VerificarLogin()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                var apiService = MauiProgram.CreateMauiApp()
                    .Services.GetService<ApiService>();
                var viewModel = new LoginViewModel(apiService);

                MainPage = new NavigationPage(new LoginPage(viewModel));
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}