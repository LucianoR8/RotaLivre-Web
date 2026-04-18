using Microsoft.AspNetCore.SignalR;

namespace Rota_LivreWEB_API.Hubs
{
    public class GrupoHub : Hub
    {
        public async Task EntrarGrupo(string grupoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);
        }

        public async Task SairGrupo(string grupoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupoId);
        }

        public async Task EnviarLocalizacao(string grupoId, object localizacao)
        {
            await Clients.Group(grupoId)
                .SendAsync("ReceberLocalizacao", localizacao);
        }

        public async Task NotificarEntrada(string grupoId, string usuario)
        {
            await Clients.Group(grupoId)
                .SendAsync("UsuarioEntrou", usuario);
        }

        public async Task NotificarSaida(string grupoId, string usuario)
        {
            await Clients.Group(grupoId)
                .SendAsync("UsuarioSaiu", usuario);
        }
    }
}