using Microsoft.Maui.Storage;
using RotaLivreMobile.Views;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;

namespace RotaLivreMobile
{
    public partial class App : Application
    {
        private string _codigoDeepLink;

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

                if (!string.IsNullOrEmpty(_codigoDeepLink))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(500);
                        await Shell.Current.GoToAsync($"//grupoDetalhe?codigo={_codigoDeepLink}");
                        _codigoDeepLink = null;
                    });
                }
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
            Console.WriteLine("DEEP LINK RECEBIDO");

            if (uri.Host == "grupo")
            {
                var codigo = System.Web.HttpUtility.ParseQueryString(uri.Query)
                    .Get("codigo");

                if (!string.IsNullOrEmpty(codigo))
                {
                    _codigoDeepLink = codigo;

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (Shell.Current != null)
                        {
                            await Shell.Current.GoToAsync($"//grupoDetalhe?codigo={codigo}");
                            _codigoDeepLink = null;
                        }
                    });
                }
            }
        }
    }

}
