using Microsoft.AspNetCore.SignalR.Client;

namespace RotaLivreMobile.Services;

public class GrupoSignalRService
{
    private HubConnection _connection;

    public event Action<string>? OnUsuarioEntrou;
    public event Action<string>? OnUsuarioSaiu;

    public async Task ConectarAsync()
    {
        if (_connection != null &&
            _connection.State == HubConnectionState.Connected)
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl("https://rotalivre-web.onrender.com/grupoHub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string>("UsuarioEntrou", usuario =>
        {
            OnUsuarioEntrou?.Invoke(usuario);
        });

        await _connection.StartAsync();
    }

    public async Task EntrarGrupo(string grupoId, string nomeUsuario)
    {
        if (_connection == null || _connection.State != HubConnectionState.Connected)
            await ConectarAsync();

        await _connection.InvokeAsync("EntrarGrupo", grupoId, nomeUsuario);
    }

    public async Task SairGrupo(string grupoId)
    {
        await _connection.InvokeAsync("SairGrupo", grupoId);
    }
}