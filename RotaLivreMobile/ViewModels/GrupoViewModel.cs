using System.Collections.ObjectModel;
using System.Windows.Input;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels;

public class GrupoViewModel : BaseViewModel
{
    private readonly GrupoSignalRService _signalR;

    public ObservableCollection<string> Usuarios { get; set; } = new();

    public string CodigoGrupo { get; set; }

    public ICommand EntrarCommand { get; }

    public GrupoViewModel(GrupoSignalRService signalR)
    {
        _signalR = signalR;

        EntrarCommand = new Command(async () => await EntrarGrupo());

        _signalR.OnUsuarioEntrou += usuario =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Usuarios.Add(usuario);
            });
        };
    }

    private async Task EntrarGrupo()
    {
        await _signalR.ConectarAsync();
        await _signalR.EntrarGrupo(CodigoGrupo);
    }
}