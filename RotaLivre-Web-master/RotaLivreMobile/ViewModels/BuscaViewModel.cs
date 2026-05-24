using System.Collections.ObjectModel;
using System.Windows.Input;
using RotaLivreMobile.Models;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.ViewModels;

public class BuscaViewModel : BaseViewModel
{
    private readonly PasseioApiService _service;

    public ObservableCollection<PasseioDto> Passeios { get; set; } = new();

    public ICommand BuscarCommand { get; }

    public BuscaViewModel(PasseioApiService service)
    {
        _service = service;

        BuscarCommand = new Command<string>(async (termo) =>
        {
            await Buscar(termo);
        });
    }

    private async Task Buscar(string termo)
    {
        Passeios.Clear();

        var resultado = await _service.BuscarPasseios(termo);

        foreach (var item in resultado)
            Passeios.Add(item);
    }
}