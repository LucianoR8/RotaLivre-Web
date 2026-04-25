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

        _signalR.OnGrupoAtualizado += grupo =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NomePasseio = grupo.NomePasseio;
                IdPasseio = grupo.PasseioId;

                Usuarios.Clear();
                foreach (var u in grupo.Usuarios)
                    Usuarios.Add(u);

                OnPropertyChanged(nameof(NomePasseio));
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

        var grupo = await _apiService.CriarGrupo(IdPasseio);

        CodigoGrupo = grupo.codigo_convite;

        OnPropertyChanged(nameof(CodigoGrupo));
        OnPropertyChanged(nameof(TemGrupoAtivo));
        OnPropertyChanged(nameof(LinkGrupo));

        var nomeUsuario = await _apiService.GetNomeUsuario();

        // Usuarios.Clear(); remove isso aqui?

        // Usuarios.Add(nomeUsuario); remove isso aqui?

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo, nomeUsuario);

        await SecureStorage.SetAsync("grupo_codigo", CodigoGrupo);
        await SecureStorage.SetAsync("grupo_nome", NomePasseio);
        await SecureStorage.SetAsync("grupo_id", IdPasseio.ToString());
    }

    private async Task EntrarGrupo()
    {
        if (string.IsNullOrWhiteSpace(CodigoDigitado))
            return;

        var nomeUsuario = await _apiService.GetNomeUsuario();

        // Usuarios.Clear();
        // Usuarios.Add(nomeUsuario);

        await _signalR.ConectarAsync();

        try
        {
            await _signalR.EntrarGrupo(CodigoGrupo, nomeUsuario);

            CodigoGrupo = CodigoDigitado;

            OnPropertyChanged(nameof(CodigoGrupo));
            OnPropertyChanged(nameof(TemGrupoAtivo));

        }
        catch (Exception)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Erro",
                "Grupo não encontrado",
                "OK");
        }

        await SecureStorage.SetAsync("grupo_codigo", CodigoGrupo);
    }

    public async Task EntrarGrupoDireto()
    {
        if (string.IsNullOrWhiteSpace(CodigoDigitado))
            return;

        CodigoGrupo = CodigoDigitado;

        OnPropertyChanged(nameof(CodigoGrupo));
        OnPropertyChanged(nameof(TemGrupoAtivo));

        var nomeUsuario = await _apiService.GetNomeUsuario();

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo, nomeUsuario);
    }

    public async Task RestaurarGrupo()
    {
        var codigo = await SecureStorage.GetAsync("grupo_codigo");

        if (!string.IsNullOrEmpty(codigo))
        {
            CodigoDigitado = codigo;
            await EntrarGrupoDireto();
        }
    }

    public async Task SairGrupo()
    {
        var nome = await _apiService.GetNomeUsuario();

        await _signalR.SairGrupo(CodigoGrupo, nome);

        CodigoGrupo = null;
        Usuarios.Clear();

        SecureStorage.Remove("grupo_codigo");

        await Shell.Current.GoToAsync($"//PasseioDetalhePage?PasseioId={IdPasseio}");
    }

}