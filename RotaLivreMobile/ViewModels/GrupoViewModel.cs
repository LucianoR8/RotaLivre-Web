using System.Collections.ObjectModel;
using System.Windows.Input;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;

namespace RotaLivreMobile.ViewModels;

public class GrupoViewModel : BaseViewModel
{
    private readonly GrupoSignalRService _signalR;
    private readonly ApiService _apiService;

    public ObservableCollection<string> Usuarios { get; set; } = new();

    public string CodigoGrupo { get; set; }

    public string NomePasseio { get; set; }
    public int IdPasseio { get; set; }

    public bool PodeCriarGrupo => IdPasseio > 0;

    public bool TemGrupoAtivo => !string.IsNullOrEmpty(CodigoGrupo);

    private string _codigoDigitado;
    public string CodigoDigitado
    {
        get => _codigoDigitado;
        set
        {
            _codigoDigitado = value;
            OnPropertyChanged();
        }
    }



    public string LinkGrupo =>
        $"https://rotalivre-web.onrender.com/grupo?codigo={CodigoGrupo}";
    public ICommand EntrarGrupoCommand { get; }

    public ICommand CriarGrupoCommand { get; }

    public GrupoViewModel(GrupoSignalRService signalR, ApiService apiService)
    {
        _signalR = signalR;
        _apiService = apiService;

        CriarGrupoCommand = new Command(async () => await CriarGrupo());
        EntrarGrupoCommand = new Command(async () => await EntrarGrupo());

        _signalR.OnUsuarioEntrou += usuario =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!Usuarios.Contains(usuario))
                    Usuarios.Add(usuario);
            });
        };

        _signalR.OnListaUsuarios += lista =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Usuarios.Clear();
                foreach (var u in lista)
                    Usuarios.Add(u);
            });
        };
    }
    private async Task CriarGrupo()
    {
        if (TemGrupoAtivo)
            return;

        if (IdPasseio <= 0)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Erro",
                "Escolha um passeio antes de criar um grupo",
                "OK");
            return;
        }

        CodigoGrupo = Guid.NewGuid().ToString().Substring(0, 6);

        OnPropertyChanged(nameof(CodigoGrupo));
        OnPropertyChanged(nameof(TemGrupoAtivo));
        OnPropertyChanged(nameof(LinkGrupo));

        var nomeUsuario = await _apiService.GetNomeUsuario();

        // Usuarios.Clear(); remove isso aqui?

        // Usuarios.Add(nomeUsuario); remove isso aqui?

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo, nomeUsuario);
    }

    private async Task EntrarGrupo()
    {
        if (string.IsNullOrWhiteSpace(CodigoDigitado))
            return;

        CodigoGrupo = CodigoDigitado;

        OnPropertyChanged(nameof(CodigoGrupo));
        OnPropertyChanged(nameof(TemGrupoAtivo));

        var nomeUsuario = await _apiService.GetNomeUsuario();

        // Usuarios.Clear();
        // Usuarios.Add(nomeUsuario);

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo, nomeUsuario);
    }
}