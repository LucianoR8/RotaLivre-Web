using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RotaLivreMobile.Services;
using RotaLivreMobile.Views;

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

    public ObservableCollection<PasseioDto> Destaques { get; set; }

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Destaques = new ObservableCollection<PasseioDto>();
    }

    public async Task CarregarHome()
    {
        var home = await _apiService.GetHome();

        if (home == null)
            return;

        NomeUsuario = home.NomeUsuario;

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