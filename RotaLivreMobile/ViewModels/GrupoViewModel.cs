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

    public string GrupoNomeSalvo { get; set; }
    public string GrupoCodigoSalvo { get; set; }
    public bool TemGrupoSalvo => !string.IsNullOrEmpty(GrupoCodigoSalvo);
    public string LinkGrupo =>
        $"https://rotalivre-web.onrender.com/grupo?codigo={CodigoGrupo}";
    public bool MostrarCardGrupoSalvo => TemGrupoSalvo && !TemGrupoAtivo;
    public bool MostrarTelaInicial => !TemGrupoAtivo && !MostrarCardGrupoSalvo;
    public ICommand EntrarGrupoCommand { get; }
    public ICommand CriarGrupoCommand { get; }
    public ICommand EntrarGrupoSalvoCommand { get; }

    public GrupoViewModel(GrupoSignalRService signalR, ApiService apiService)
    {
        _signalR = signalR;
        _apiService = apiService;

        CriarGrupoCommand = new Command(async () => await CriarGrupo());
        EntrarGrupoCommand = new Command(async () => await EntrarGrupo());
        EntrarGrupoSalvoCommand = new Command(async () => await EntrarGrupoSalvo());

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

        _signalR.OnErroGrupo += mensagem =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Erro", mensagem, "OK");

                CodigoGrupo = null;
                CodigoDigitado = null;

                OnPropertyChanged(nameof(CodigoGrupo));
                OnPropertyChanged(nameof(CodigoDigitado));
                AtualizarEstados();
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
        OnPropertyChanged(nameof(LinkGrupo));
        AtualizarEstados();

        var nomeUsuario = await _apiService.GetNomeUsuario();

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

        IsLoading = true;

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoDigitado, nomeUsuario);

        CodigoGrupo = CodigoDigitado;
        CodigoDigitado = null;

        OnPropertyChanged(nameof(CodigoGrupo));
        AtualizarEstados();

        await SecureStorage.SetAsync("grupo_codigo", CodigoGrupo);

        IsLoading = false;
    }

    public async Task EntrarGrupoDireto()
    {
        if (string.IsNullOrWhiteSpace(CodigoDigitado))
            return;

        CodigoGrupo = CodigoDigitado;

        OnPropertyChanged(nameof(CodigoGrupo));
        AtualizarEstados();

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
    private async Task EntrarGrupoSalvo()
    {
        if (string.IsNullOrEmpty(GrupoCodigoSalvo))
            return;

        CodigoDigitado = GrupoCodigoSalvo;

        await EntrarGrupo();
    }
    public async Task CarregarGrupoSalvo()
    {
        GrupoCodigoSalvo = await SecureStorage.GetAsync("grupo_codigo");
        GrupoNomeSalvo = await SecureStorage.GetAsync("grupo_nome");

        if (!string.IsNullOrEmpty(GrupoCodigoSalvo))
        {
            var ativo = await _apiService.GrupoExisteEAtivo(GrupoCodigoSalvo);

            if (!ativo)
            {
                SecureStorage.Remove("grupo_codigo");
                SecureStorage.Remove("grupo_nome");

                GrupoCodigoSalvo = null;
                GrupoNomeSalvo = null;
            }
        }

        OnPropertyChanged(nameof(GrupoCodigoSalvo));
        OnPropertyChanged(nameof(GrupoNomeSalvo));
        AtualizarEstados();
    }

    public async Task SairGrupo()
    {
        var nome = await _apiService.GetNomeUsuario();

        await _signalR.SairGrupo(CodigoGrupo, nome);

        CodigoGrupo = null;
        Usuarios.Clear();

        // SecureStorage.Remove("grupo_codigo");
        // SecureStorage.Remove("grupo_nome");
        // SecureStorage.Remove("grupo_id");

        await Shell.Current.GoToAsync("//HomePage");

        AtualizarEstados();
    }

    private void AtualizarEstados()
    {
        OnPropertyChanged(nameof(TemGrupoAtivo));
        OnPropertyChanged(nameof(TemGrupoSalvo));
        OnPropertyChanged(nameof(MostrarCardGrupoSalvo));
        OnPropertyChanged(nameof(MostrarTelaInicial));
    }

}