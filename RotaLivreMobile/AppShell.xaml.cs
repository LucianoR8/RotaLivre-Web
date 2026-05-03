using RotaLivreMobile.Views;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;
using RotaLivreMobile.Models;

namespace RotaLivreMobile
{
    public partial class AppShell : Shell
    {
        private string _ultimoCodigoProcessado;
        private IDispatcherTimer _clipboardTimer;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("detalhe", typeof(PasseioDetalhePage));
            Routing.RegisterRoute(nameof(CategoriaPage), typeof(CategoriaPage));
            Routing.RegisterRoute(nameof(RealizarPasseioPage), typeof(RealizarPasseioPage));
            Routing.RegisterRoute("grupo", typeof(GrupoPage));
            Routing.RegisterRoute("grupoDetalhe", typeof(GrupoPage));
            Routing.RegisterRoute(nameof(MeusPasseiosPage), typeof(MeusPasseiosPage));

            IniciarMonitoramentoClipboard();
        }

        private void IniciarMonitoramentoClipboard()
        {
            _clipboardTimer = Dispatcher.CreateTimer();

            _clipboardTimer.Interval = TimeSpan.FromSeconds(2);

            _clipboardTimer.Tick += async (s, e) =>
            {
                await VerificarCodigoClipboard();
            };

            _clipboardTimer.Start();
        }

        private async Task VerificarCodigoClipboard()
        {
            try
            {
                var texto = await Clipboard.GetTextAsync();

                if (string.IsNullOrWhiteSpace(texto))
                    return;

                if (texto.Contains("codigo="))
                {
                    texto = texto.Split("codigo=").Last();
                }

                if (!EhCodigoValido(texto))
                    return;

                if (texto == _ultimoCodigoProcessado)
                    return;

                _ultimoCodigoProcessado = texto;

                bool entrar = await Application.Current.MainPage.DisplayAlert(
                    "Convite de grupo",
                    $"Deseja entrar no grupo com código {texto}?",
                    "Entrar",
                    "Cancelar");

                if (entrar)
                {
                    await EntrarNoGrupoViaClipboard(texto);
                }
            }
            catch
            {

            }
        }

        private async Task EntrarNoGrupoViaClipboard(string codigo)
        {
            try
            {
                var page = Shell.Current.CurrentPage;

                if (page?.BindingContext is GrupoViewModel vm)
                {
                    vm.CodigoDigitado = codigo;
                    await vm.EntrarGrupo();
                }
                else
                {
                    await Shell.Current.GoToAsync("grupo");

                    await Task.Delay(500);

                    var novaPage = Shell.Current.CurrentPage;

                    if (novaPage?.BindingContext is GrupoViewModel novoVm)
                    {
                        novoVm.CodigoDigitado = codigo;
                        await novoVm.EntrarGrupo();
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private bool EhCodigoValido(string texto)
        {
            return texto.Length == 6 && texto.All(char.IsLetterOrDigit);
        }
    }
}