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
    public string ConfirmarSenha { get; set; }
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

    private DateTime _dataSelecionada = DateTime.Today;

    public DateTime DataSelecionada
    {
        get => _dataSelecionada;
        set
        {
            _dataSelecionada = value;
            OnPropertyChanged();
        }
    }

    public ICommand CadastrarCommand { get; }

    private async Task Cadastrar()
    {


        if (string.IsNullOrWhiteSpace(Nome) ||
        string.IsNullOrWhiteSpace(Email) ||
        string.IsNullOrWhiteSpace(Senha) ||
        string.IsNullOrWhiteSpace(ConfirmarSenha) ||
        string.IsNullOrWhiteSpace(RespostaSeg))
        {
            await Shell.Current.DisplayAlert("Erro", "Preencha todos os campos", "OK");
            return;
        }

        if (Senha != ConfirmarSenha)
        {
            await Shell.Current.DisplayAlert("Erro", "As senhas não coincidem", "OK");
            return;
        }

        if (PerguntaSelecionada == null)
        {
            await Shell.Current.DisplayAlert("Erro", "Selecione uma pergunta de segurança", "OK");
            return;
        }

        var dto = new UsuarioCadastroDto
        {
            Nome = Nome,
            DataNasc = DataSelecionada.ToString("yyyy-MM-dd"),
            Email = Email,
            Senha = Senha,
            RespostaSeg = RespostaSeg,
            IdPergunta = PerguntaSelecionada.Id_Pergunta
        };

        var sucesso = await _usuarioService.CadastrarUsuario(dto);

        if (sucesso)
        {
            await Application.Current.MainPage.DisplayAlert("Sucesso", "Cadastro realizado!", "OK");

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

        if (lista == null || lista.Count == 0)
            return;

        Perguntas.Clear();

        Console.WriteLine($"TOTAL: {Perguntas.Count}");

        foreach (var p in lista)
        {
            Console.WriteLine($"ID: {p.Id_Pergunta} - Pergunta: {p.pergunta_seg}");
        }

        foreach (var p in lista)
        {
            Perguntas.Add(p);
        }
        Console.WriteLine($"TOTAL DEPOIS: {Perguntas.Count}");

        OnPropertyChanged(nameof(Perguntas));
    }
}