using Microsoft.AspNetCore.SignalR;
using Rota_LivreWEB_API.DTOs.Grupo;

namespace Rota_LivreWEB_API.Hubs
{
    public class GrupoHub : Hub
    {
        private static Dictionary<string, GrupoInfo> Grupos = new();

        public class GrupoInfo
        {
            public int PasseioId { get; set; }
            public string NomePasseio { get; set; }
            public List<string> Usuarios { get; set; } = new();
        }

        public async Task EntrarGrupo(string grupoId, string nomeUsuario, int passeioId, string nomePasseio)
        {
            if (!Grupos.ContainsKey(grupoId))
            {
                Grupos[grupoId] = new GrupoInfo
                {
                    PasseioId = passeioId,
                    NomePasseio = nomePasseio
                };
            }

            var grupo = Grupos[grupoId];

            if (!Grupos.ContainsKey(grupoId))
            {
                Grupos[grupoId] = new GrupoInfo
                {
                    PasseioId = passeioId,
                    NomePasseio = nomePasseio
                };
            }

            if (!grupo.Usuarios.Contains(nomeUsuario))
                grupo.Usuarios.Add(nomeUsuario);

            await Groups.AddToGroupAsync(Context.ConnectionId, grupoId);

            await Clients.Group(grupoId)
                .SendAsync("GrupoAtualizado", grupo);
        }

        public async Task SairGrupo(string grupoId, string usuario)
        {
            if (Grupos.ContainsKey(grupoId))
            {
                var grupo = Grupos[grupoId];

                grupo.Usuarios.Remove(usuario);

                await Clients.Group(grupoId)
                    .SendAsync("GrupoAtualizado", grupo);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupoId);
        }

        public async Task EnviarLocalizacao(string grupoId, LocalizacaoDto localizacao)
        {
            await Clients.Group(grupoId)
                .SendAsync("ReceberLocalizacao", localizacao);
        }
    }
}