using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views;

public partial class MeusPasseiosPage : ContentPage
{
    private MeusPasseiosViewModel _viewModel;

    public MeusPasseiosPage(MeusPasseiosViewModel vm)
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
}