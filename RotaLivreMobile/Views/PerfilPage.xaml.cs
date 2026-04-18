using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();
        BindingContext = new PerfilViewModel();
    }
}