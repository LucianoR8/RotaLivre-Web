using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

public partial class PerfilPage : ContentPage
{
    private readonly PerfilViewModel _viewModel;

    public PerfilPage(PerfilViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.CarregarCommand.CanExecute(null))
            _viewModel.CarregarCommand.Execute(null);
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EditarPerfilPage));
    }


}