using RotaLivreMobile.Views;
using RotaLivreMobile.ViewModels;
using RotaLivreMobile.Services;

namespace RotaLivreMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("detalhe", typeof(PasseioDetalhePage));
            Routing.RegisterRoute(nameof(CategoriaPage), typeof(CategoriaPage));
            Routing.RegisterRoute(nameof(RealizarPasseioPage), typeof(RealizarPasseioPage));
            Routing.RegisterRoute("grupo", typeof(GrupoPage));
            Routing.RegisterRoute("grupoDetalhe", typeof(GrupoPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(DeepLinkService.CodigoGrupo))
            {
                var codigo = DeepLinkService.CodigoGrupo;
                DeepLinkService.CodigoGrupo = null;

                await Task.Delay(500);

                await Shell.Current.GoToAsync("grupoDetalhe");

                var page = Shell.Current.CurrentPage as GrupoPage;

                if (page?.BindingContext is GrupoViewModel vm)
                {
                    vm.CodigoDigitado = codigo;
                    await vm.EntrarGrupoDireto();
                }
            }
        }


    }
}
