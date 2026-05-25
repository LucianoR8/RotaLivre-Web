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
            var grupo = await _context.Grupo
                .Include(g => g.Passeio)
                .FirstOrDefaultAsync(g => g.codigo_convite == codigo);

            if (grupo == null)
            {
                await Clients.Caller.SendAsync("ErroGrupo", "Grupo não existe");
                return;
            }

            if (grupo.status == "FINALIZADO")
            {
                await Clients.Caller.SendAsync("ErroGrupo", "Grupo encerrado");
                return;
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.nome_completo == nomeUsuario);

            if (usuario == null)
            {
                await Clients.Caller.SendAsync("Usuário não encontrado");
                return;
            }

            var totalUsuarios = await _context.GrupoUsuario
                .CountAsync(x => x.id_grupo == grupo.id_grupo && x.ativo);

            if (totalUsuarios >= 10)
            {
                await Clients.Caller.SendAsync("Grupo cheio");
                return;
            }

            var relacao = await _context.GrupoUsuario
                .FirstOrDefaultAsync(x => x.id_grupo == grupo.id_grupo && x.id_usuario == usuario.id_usuario);

            if (relacao != null)
            {
                relacao.ativo = true;
            }
            else
            {
                _context.GrupoUsuario.Add(new GrupoUsuario
                {
                    id_grupo = grupo.id_grupo,
                    id_usuario = usuario.id_usuario,
                    ativo = true
                });
            }

            await _context.SaveChangesAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, codigo);

            var usuarios = await _context.GrupoUsuario
                .Include(x => x.Usuario)
                .Where(x => x.id_grupo == grupo.id_grupo && x.ativo)
                .Select(x => x.Usuario.nome_completo)
                .ToListAsync();

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

            var usuariosAtivos = _context.GrupoUsuario
                .Count(x => x.id_grupo == grupo.id_grupo && x.ativo);

            if (usuariosAtivos == 0)
            {
                grupo.status = "FINALIZADO";
                await _context.SaveChangesAsync();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, codigo);

            var usuarios = _context.GrupoUsuario
                .Include(x => x.Usuario)
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