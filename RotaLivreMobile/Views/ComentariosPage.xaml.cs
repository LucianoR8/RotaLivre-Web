using RotaLivreMobile.Models;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

[QueryProperty(nameof(IdPasseio), "idPasseio")]
public partial class ComentariosPage : ContentPage
{
    private readonly AvaliacaoApiService _service;

    public int IdPasseio { get; set; }

    public ComentariosPage(AvaliacaoApiService service)
    {
        InitializeComponent();

        _service = service;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarComentarios();
    }

    private async Task CarregarComentarios()
    {
        var lista =
            await _service.ListarAsync(IdPasseio);

        comentariosView.ItemsSource = lista;
    }

    private async void OnEnviarClicked(
        object sender,
        EventArgs e)
    {
        var texto =
            comentarioEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(texto))
            return;

        int idUsuario =
            Preferences.Get("IdUsuario", 0);

        await DisplayAlert(
            "DEBUG",
            $"IdUsuario: {idUsuario}",
            "OK");

        var dto = new CriarAvaliacaoDto
        {
            IdPasseio = IdPasseio,
            IdUsuario = idUsuario,
            Feedback = texto
        };

        var sucesso =
            await _service.ComentarAsync(dto);

        if (!sucesso)
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível comentar",
                "OK");

            return;
        }

        comentarioEntry.Text = "";

        await CarregarComentarios();
    }
}