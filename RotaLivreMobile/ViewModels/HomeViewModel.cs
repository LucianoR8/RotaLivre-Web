using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RotaLivreMobile.Services;
using RotaLivreMobile.Views;
using RotaLivreMobile.Models;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    private string _nomeUsuario;
    public string NomeUsuario
    {
        get => _nomeUsuario;
        set
        {
            _nomeUsuario = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CategoriaDto> Categorias { get; set; }
    public ObservableCollection<PasseioDto> Destaques { get; set; }

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Categorias = new ObservableCollection<CategoriaDto>();
        Destaques = new ObservableCollection<PasseioDto>();
    }

    public async Task CarregarHome()
    {
        var home = await _apiService.GetHome();

        if (home == null)
            return;

        NomeUsuario = home.NomeUsuario;

        Categorias.Clear();
        foreach (var c in home.Categorias)
            Categorias.Add(c);

        Destaques.Clear();
        foreach (var item in home.Destaques)
            Destaques.Add(item);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}