using Microsoft.AspNetCore.SignalR;
using Rota_LivreWEB_API.DTOs.Grupo;

namespace Rota_LivreWEB_API.Hubs
{
    public class GrupoHub : Hub
    {
        private static Dictionary<string, List<string>> GruposUsuarios = new();

        public async Task EntrarGrupo(string grupoId, string nomeUsuario)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);

            if (!GruposUsuarios.ContainsKey(grupoId))
                GruposUsuarios[grupoId] = new List<string>();

            if (!GruposUsuarios[grupoId].Contains(nomeUsuario))
                GruposUsuarios[grupoId].Add(nomeUsuario);

            await Clients.Group(grupoId)
                .SendAsync("ListaUsuarios", GruposUsuarios[grupoId]);
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