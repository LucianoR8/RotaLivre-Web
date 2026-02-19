using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CarregarHome();
        }

        private async void OnPasseioSelecionado(object sender, SelectionChangedEventArgs e)
        {
            var passeio = e.CurrentSelection.FirstOrDefault() as PasseioDto;

            if (passeio == null)
                return;

            await Shell.Current.GoToAsync("detalhe",
    new Dictionary<string, object>
    {
        { "PasseioId", passeio.Id }
    });

        }
    }
}