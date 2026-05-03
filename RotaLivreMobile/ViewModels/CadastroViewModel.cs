using RotaLivreMobile.Models;
using RotaLivreMobile.Services;
using RotaLivreMobile.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class CadastroViewModel : BaseViewModel
{
    private readonly UsuarioApiService _usuarioService;

    public CadastroViewModel(UsuarioApiService usuarioService)
    {
        _usuarioService = usuarioService;
        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    public string Nome { get; set; }
    public string DataNasc { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string RespostaSeg { get; set; }


    public ObservableCollection<PerguntaSegurancaDto> Perguntas { get; set; } = new();

    private PerguntaSegurancaDto _perguntaSelecionada;
    public PerguntaSegurancaDto PerguntaSelecionada
    {
        get => _perguntaSelecionada;
        set
        {
            _perguntaSelecionada = value;
            OnPropertyChanged();
        }
    }

    public ICommand CadastrarCommand { get; }

    private async Task Cadastrar()
    {
        var dto = new UsuarioCadastroDto
        {
            Nome = Nome,
            DataNasc = DataNasc,
            Email = Email,
            Senha = Senha,
            RespostaSeg = RespostaSeg,
            IdPergunta = PerguntaSelecionada != null
        ? PerguntaSelecionada.Id_Pergunta
        : 0
        };

        var sucesso = await _usuarioService.CadastrarUsuario(dto);

        if (sucesso)
        {
            await Shell.Current.DisplayAlert("Sucesso", "Cadastro realizado!", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else
        {
            await Shell.Current.DisplayAlert("Erro", "Falha ao cadastrar", "OK");
        }
    }

    public async Task CarregarPerguntas()
    {
        var lista = await _usuarioService.GetPerguntasAsync();

        Perguntas.Clear();

        foreach (var p in lista)
            Perguntas.Add(p);
    }
}