using Microsoft.Maui.Storage;
using RotaLivreMobile.Views;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;

namespace RotaLivreMobile
{
    public partial class App : Application
    {
        public string CodigoDeepLink;

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            InicializarApp();
        }

        private async void InicializarApp()
        {
            await Task.Delay(300); 

            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//HomePage");
            }
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

                if (!string.IsNullOrEmpty(CodigoDeepLink))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(1000);
                        await Shell.Current.GoToAsync($"grupoDetalhe?codigo={CodigoDeepLink}");
                        CodigoDeepLink = null;
                    });
                }
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if (uri.Host == "grupo")
            {
                var codigo = System.Web.HttpUtility.ParseQueryString(uri.Query)
                    .Get("codigo");

                if (!string.IsNullOrEmpty(codigo))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.GoToAsync($"//grupoDetalhe?codigo={codigo}");
                    });
                }
            }
        }
    }

}
