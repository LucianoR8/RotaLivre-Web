using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

public partial class EditarPerfilPage : ContentPage
{
    private readonly EditarPerfilViewModel _viewModel;

    public EditarPerfilPage(EditarPerfilViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.CarregarCommand.Execute(null);
    }
}