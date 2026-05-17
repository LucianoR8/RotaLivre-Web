using RotaLivreMobile.ViewModels;
using Microsoft.Maui.Media;
using System.Net.Http.Headers;
using System.Text.Json;
using RotaLivreMobile.Services;

namespace RotaLivreMobile.Views;

public partial class EditarPerfilPage : ContentPage
{
    private readonly EditarPerfilViewModel _viewModel;
    private readonly UsuarioApiService _service;

    public EditarPerfilPage(
    EditarPerfilViewModel vm,
    UsuarioApiService service)
    {
        InitializeComponent();

        _service = service;

        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.CarregarCommand.Execute(null);
    }

    private async void OnSelecionarFotoTapped(object sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();

            if (foto == null)
                return;

            var id = await SecureStorage.GetAsync("usuario_id");

            if (!int.TryParse(id, out int usuarioId))
                return;

            var (sucesso, fotoUrl, erro) =
                await _service.UploadFotoPerfil(usuarioId, foto);

            if (!sucesso)
            {
                await DisplayAlert("Erro", erro, "OK");
                return;
            }

            _viewModel.FotoPerfilUrl = fotoUrl;

            await DisplayAlert(
                "Sucesso",
                "Foto atualizada!",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnRemoverFotoClicked(object sender, EventArgs e)
    {
        try
        {
            var confirm = await DisplayAlert(
                "Remover foto",
                "Deseja remover sua foto de perfil?",
                "Sim",
                "Cancelar");

            if (!confirm)
                return;

            var id = await SecureStorage.GetAsync("usuario_id");

            if (!int.TryParse(id, out int usuarioId))
                return;

            var (sucesso, erro) =
                await _service.RemoverFotoPerfil(usuarioId);

            if (!sucesso)
            {
                await DisplayAlert("Erro", erro, "OK");
                return;
            }

            _viewModel.FotoPerfilUrl = null;

            await DisplayAlert(
                "Sucesso",
                "Foto removida!",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

}