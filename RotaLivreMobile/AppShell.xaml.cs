using RotaLivreMobile.Views;

namespace RotaLivreMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("detalhe", typeof(PasseioDetalhePage));
        }


    }
}
