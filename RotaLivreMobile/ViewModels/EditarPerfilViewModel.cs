using System.Windows.Input;
using Microsoft.Maui.Storage;
using RotaLivreMobile.Models;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;

public class EditarPerfilViewModel : BaseViewModel
{
    private readonly UsuarioApiService _service;

    public int Id { get; set; }

    private string _nome;
    public string Nome
    {
        get => _nome;
        set
        {
            _nome = value;
            OnPropertyChanged();
        }
    }
    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }
    private DateTime _datanasc;
    public DateTime DataNasc
    {
        get => _datanasc;
        set
        {
            _datanasc = value;
            OnPropertyChanged();
        }
    }
    

    public ICommand CarregarCommand { get; }
    public ICommand SalvarCommand { get; }

    public EditarPerfilViewModel(UsuarioApiService service)
    {
        _service = service;

        CarregarCommand = new Command(async () => await Carregar());
        SalvarCommand = new Command(async () => await Salvar());
    }

    private async Task Carregar()
    {
        var id = await SecureStorage.GetAsync("usuario_id");

        if (!int.TryParse(id, out int userId))
            return;

        var perfil = await _service.GetPerfil(userId);

        if (perfil == null) return;

        Id = perfil.Id;
        Nome = perfil.Nome;
        Email = perfil.Email;
        DataNasc = DateTime.Parse(perfil.DataNasc);

        OnPropertyChanged(nameof(Nome));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(DataNasc));
    }

    private async Task Salvar()
    {
        var dto = new UsuarioPerfilDto
        {
            Id = Id,
            Nome = Nome,
            Email = Email,
            DataNasc = DataNasc.ToString("yyyy-MM-dd")
        };

        var (sucesso, erro) = await _service.AtualizarPerfil(dto);

        if (!sucesso)
        {
            if (erro.Contains("EMAIL_JA_EXISTE"))
            {
                await Shell.Current.DisplayAlert("Erro", "Email já está em uso", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Erro", erro, "OK");
            return;
        }

        await Shell.Current.DisplayAlert("Sucesso", "Perfil atualizado!", "OK");

        await Shell.Current.GoToAsync(".."); 
    }
}