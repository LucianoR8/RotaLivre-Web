using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;
using System.Windows.Input;

public class PerfilViewModel : BaseViewModel
{
    private readonly UsuarioApiService _service;

    public string Nome { get; set; }
    public string Email { get; set; }
    public string DataNasc { get; set; }

    public ICommand CarregarCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand DeletarCommand { get; }

    public PerfilViewModel(UsuarioApiService service)
    {
        _service = service;

        CarregarCommand = new Command(async () => await Carregar());
        LogoutCommand = new Command(async () => await Logout());
        DeletarCommand = new Command(async () => await Deletar());
    }

    private async Task Carregar()
    {
        var id = await SecureStorage.GetAsync("usuario_id");

        if (!int.TryParse(id, out int userId))
            return;

        var perfil = await _service.GetPerfil(userId);

        if (perfil == null) return;

        Nome = perfil.Nome;
        Email = perfil.Email;
        DataNasc = perfil.DataNasc;

        OnPropertyChanged(nameof(Nome));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(DataNasc));
    }

    private async Task Logout()
    {
        SecureStorage.Remove("auth_token");
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async Task Deletar()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Excluir conta",
            "Tem certeza?",
            "Sim",
            "Cancelar");

        if (!confirm) return;

        var id = await SecureStorage.GetAsync("usuario_id");

        if (!int.TryParse(id, out int userId))
            return;

        var sucesso = await _service.DeletarConta(userId);

        if (sucesso)
        {
            SecureStorage.RemoveAll();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}