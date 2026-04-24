using Microsoft.AspNetCore.SignalR;
using Rota_LivreWEB_API.DTOs.Grupo;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Rota_LivreWEB_API.Hubs
{
    public class GrupoHub : Hub
    {
        private readonly AppDbContext _context;

        public GrupoHub(AppDbContext context)
        {
            _context = context;
        }
        public class GrupoInfo
        {
            public int PasseioId { get; set; }
            public string NomePasseio { get; set; }
            public List<string> Usuarios { get; set; } = new();
        }

        public async Task EntrarGrupo(string codigo, string nomeUsuario)
        {
            var grupo = _context.Grupo
                .Include(g => g.Passeio)
                .Include(g => g.Usuarios)
                .FirstOrDefault(g => g.codigo_convite == codigo);

            if (grupo == null)
                throw new Exception("Grupo não existe");

            if (grupo.status == "FINALIZADO")
                throw new Exception("Grupo encerrado");

            var usuario = _context.Usuario
                .FirstOrDefault(u => u.nome_completo == nomeUsuario);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var jaExiste = _context.GrupoUsuario
                .Any(x => x.id_grupo == grupo.id_grupo && x.id_usuario == usuario.id_usuario);

            if (!jaExiste)
            {
                _context.GrupoUsuario.Add(new GrupoUsuario
                {
                    id_grupo = grupo.id_grupo,
                    id_usuario = usuario.id_usuario
                });

                await _context.SaveChangesAsync();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, codigo);

            var usuarios = _context.GrupoUsuario
                .Where(x => x.id_grupo == grupo.id_grupo && x.ativo)
                .Select(x => x.Usuario.nome_completo)
                .ToList();

            await Clients.Group(codigo).SendAsync("GrupoAtualizado", new
            {
                PasseioId = grupo.id_passeio,
                NomePasseio = grupo.Passeio.nome_passeio,
                Usuarios = usuarios
            });
        }
        public async Task SairGrupo(string codigo, string nomeUsuario)
        {
            var grupo = _context.Grupo
                .Include(g => g.Passeio)
                .FirstOrDefault(g => g.codigo_convite == codigo);

            if (grupo == null) return;

            var usuario = _context.Usuario
                .FirstOrDefault(u => u.nome_completo == nomeUsuario);

            if (usuario == null) return;

            var relacao = _context.GrupoUsuario
                .FirstOrDefault(x => x.id_grupo == grupo.id_grupo && x.id_usuario == usuario.id_usuario);

            if (relacao != null)
            {
                relacao.ativo = false;
                await _context.SaveChangesAsync();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, codigo);

            var usuarios = _context.GrupoUsuario
                .Where(x => x.id_grupo == grupo.id_grupo && x.ativo)
                .Select(x => x.Usuario.nome_completo)
                .ToList();

            await Clients.Group(codigo).SendAsync("GrupoAtualizado", new
            {
                PasseioId = grupo.id_passeio,
                NomePasseio = grupo.Passeio.nome_passeio,
                Usuarios = usuarios
            });
        }
        public async Task EnviarLocalizacao(string grupoId, LocalizacaoDto localizacao)
        {
            await Clients.Group(grupoId)
                .SendAsync("ReceberLocalizacao", localizacao);
        }
    }
}