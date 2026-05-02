using RotaLivreMobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RotaLivreMobile.ViewModels;

public class PasseioDetalheViewModel : INotifyPropertyChanged
{
    private readonly PasseioApiService _service;

    public event PropertyChangedEventHandler PropertyChanged;

    private int _id;
    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    private string _nome;
    public string Nome
    {
        get => _nome;
        set { _nome = value; OnPropertyChanged(); }
    }

    private string _descricao;
    public string Descricao
    {
        get => _descricao;
        set { _descricao = value; OnPropertyChanged(); }
    }

    private string _funcionamento;
    public string Funcionamento
    {
        get => _funcionamento;
        set { _funcionamento = value; OnPropertyChanged(); }
    }

    private string _imagemUrl;
    public string ImagemUrl
    {
        get => _imagemUrl;
        set { _imagemUrl = value; OnPropertyChanged(); }
    }

    private bool _curtido;
    public bool Curtido
    {
        get => _curtido;
        set
        {
            _curtido = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TextoBotaoCurtir));
            OnPropertyChanged(nameof(CorBotaoCurtir));
        }
    }

    private int _quantidadeCurtidas;
    public int QuantidadeCurtidas
    {
        get => _quantidadeCurtidas;
        set
        {
            _quantidadeCurtidas = value;
            OnPropertyChanged();
        }
    }

    public string TextoBotaoCurtir => Curtido ? "Curtido" : "Curtir";
    public Color CorBotaoCurtir => Curtido ? Colors.Red : Colors.Gray;

    public PasseioDetalheViewModel(PasseioApiService service)
    {
        _service = service;
    }

    public async Task Carregar(int id)
    {
        var passeio = await _service.GetByIdAsync(id);

        Console.WriteLine($"Buscando passeio ID: {id}");

        if (passeio == null)
        {
            Console.WriteLine("Passeio veio NULL");
            return;
        }

        if (passeio == null)
            return;

        Id = passeio.Id;
        Nome = passeio.Nome;
        Descricao = passeio.Descricao;
        Funcionamento = passeio.Funcionamento;
        ImagemUrl = passeio.ImagemUrl;
        Curtido = passeio.UsuarioJaCurtiu;
        QuantidadeCurtidas = passeio.QuantidadeCurtidas;


    }

    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public async Task Inicializar(int id)
    {
        var passeio = await _service.GetByIdAsync(id);

        if (passeio == null)
            return;

        Nome = passeio.Nome;
        Descricao = passeio.Descricao;
        Funcionamento = passeio.Funcionamento;
        ImagemUrl = passeio.ImagemUrl;
    }

    public ICommand CurtirCommand => new Command(async () => await Curtir());

    private async Task Curtir()
    {
        var result = await _service.CurtirAsync(Id);

        if (result == null)
            return;

        Curtido = result.Curtiu;
        QuantidadeCurtidas = result.TotalCurtidas;
    }

}