using RotaLivreMobile.Views;

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
        }


    }
}
