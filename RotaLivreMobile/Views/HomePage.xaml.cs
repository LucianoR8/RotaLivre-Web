using RotaLivreMobile.Models;

namespace RotaLivreMobile.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        private int _currentIndex = 0;
        private IDispatcherTimer _timer;

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += OnTimerTick;
        }

        private async void OnCategoriaClicada(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not CategoriaDto categoria)
                return;

            await Shell.Current.GoToAsync(
                $"{nameof(CategoriaPage)}?categoriaId={categoria.IdCategoria}&categoriaNome={Uri.EscapeDataString(categoria.TipoCategoria)}");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.CarregarHome();

            _currentIndex = 0;

            if (_viewModel.Categorias?.Any() == true)
                _timer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_viewModel.Categorias == null || !_viewModel.Categorias.Any())
                return;

            _currentIndex++;

            if (_currentIndex >= _viewModel.Categorias.Count)
                _currentIndex = 0;

            carouselCategorias.Position = _currentIndex;
        }

        private async void OnPasseioSelecionado(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var passeio = e.CurrentSelection[0] as PasseioDto;

            if (passeio == null)
                return;

            await Shell.Current.GoToAsync("detalhe",
                new Dictionary<string, object>
                {
            { "PasseioId", passeio.Id }
                });

            if (sender is CollectionView collection)
                collection.SelectedItem = null;
        }
    }
}