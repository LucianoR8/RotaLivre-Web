using System.Collections.ObjectModel;
using System.Windows.Input;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels;

public class GrupoViewModel : BaseViewModel
{
    private readonly GrupoSignalRService _signalR;

    public ObservableCollection<string> Usuarios { get; set; } = new();

    public string CodigoGrupo { get; set; }

    public string NomePasseio { get; set; }
    public int IdPasseio { get; set; }

    public bool TemGrupoAtivo => !string.IsNullOrEmpty(CodigoGrupo);

    public ICommand CriarGrupoCommand { get; }

    public GrupoViewModel(GrupoSignalRService signalR)
    {
        _signalR = signalR;

        CriarGrupoCommand = new Command(async () => await CriarGrupo());

        _signalR.OnUsuarioEntrou += usuario =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Usuarios.Add(usuario);
            });
        };
    }

    private async Task CriarGrupo()
    {
        CodigoGrupo = Guid.NewGuid().ToString().Substring(0, 6);

        OnPropertyChanged(nameof(CodigoGrupo));
        OnPropertyChanged(nameof(TemGrupoAtivo));

        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo);

        Usuarios.Clear();
        Usuarios.Add("Você");
    }
}