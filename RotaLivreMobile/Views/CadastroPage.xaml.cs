namespace RotaLivreMobile.Views;

public partial class CadastroPage : ContentPage
{
    private readonly CadastroViewModel _viewModel;

    public CadastroPage(CadastroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(200);
        await _viewModel.CarregarPerguntas();
    }
}