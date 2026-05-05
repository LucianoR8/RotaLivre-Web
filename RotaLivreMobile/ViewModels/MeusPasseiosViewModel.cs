using RotaLivreMobile.Models;
using RotaLivreMobile.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RotaLivreMobile.ViewModels;

public class MeusPasseiosViewModel : BaseViewModel
{
    private readonly PasseioApiService _service;

    public ObservableCollection<PasseioDto> Curtidos { get; set; } = new();
    public ObservableCollection<PasseioDto> Pendentes { get; set; } = new();

    private bool _isCurtidos = true;
    public bool IsCurtidos
    {
        get => _isCurtidos;
        set
        {
            _isCurtidos = value;
            OnPropertyChanged();
        }
    }

    public ICommand CarregarCommand { get; }
    public ICommand MostrarCurtidosCommand { get; }
    public ICommand MostrarPendentesCommand { get; }
    public ICommand AbrirDetalheCommand { get; }

    public MeusPasseiosViewModel(PasseioApiService service)
    {
        _service = service;

        CarregarCommand = new Command(async () => await Carregar());
        MostrarCurtidosCommand = new Command(() => IsCurtidos = true);
        MostrarPendentesCommand = new Command(() => IsCurtidos = false);
        AbrirDetalheCommand = new Command<int>(async (id) =>
        {
            await Shell.Current.GoToAsync($"detalhe?id={id}");
        });
    }

    private async Task Carregar()
    {
        var response = await _service.GetMeusPasseiosAsync();

        if (response == null) return;

        Curtidos.Clear();
        foreach (var item in response.Curtidos)
            Curtidos.Add(item);

        Pendentes.Clear();
        foreach (var item in response.Pendentes)
            Pendentes.Add(item);
    }
}