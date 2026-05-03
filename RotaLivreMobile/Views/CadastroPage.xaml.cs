namespace RotaLivreMobile.Views;

public partial class CadastroPage : ContentPage
{
	public CadastroPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CadastroViewModel vm)
            await vm.CarregarPerguntas();
    }
}