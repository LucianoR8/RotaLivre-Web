using RotaLivreMobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RotaLivreMobile.ViewModels;

public class PasseioDetalheViewModel : INotifyPropertyChanged
{
    private readonly PasseioApiService _service;

    public event PropertyChangedEventHandler PropertyChanged;

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

    public PasseioDetalheViewModel(PasseioApiService service)
    {
        _service = service;
    }

    public async Task Carregar(int id)
    {
        var passeio = await _service.GetByIdAsync(id);

        if (passeio == null)
            return;

        Nome = passeio.Nome;
        Descricao = passeio.Descricao;
        Funcionamento = passeio.Funcionamento;
        ImagemUrl = passeio.ImagemUrl;
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

}