using Microsoft.AspNetCore.SignalR;
using Rota_LivreWEB_API.DTOs.Grupo;

namespace Rota_LivreWEB_API.Hubs
{
    public class GrupoHub : Hub
    {
        public async Task EntrarGrupo(string grupoId, string usuarioNome)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);

            await Clients.Group(grupoId)
                .SendAsync("UsuarioEntrou", usuarioNome);
        }

        public async Task SairGrupo(string grupoId, string usuarioNome)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupoId);

            await Clients.Group(grupoId)
                .SendAsync("UsuarioSaiu", usuarioNome);
        }

        public async Task EnviarLocalizacao(string grupoId, LocalizacaoDto localizacao)
        {
            await Clients.Group(grupoId)
                .SendAsync("ReceberLocalizacao", localizacao);
        }
    }
}