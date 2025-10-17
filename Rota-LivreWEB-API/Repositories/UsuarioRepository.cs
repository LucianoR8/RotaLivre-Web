using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Data;
using Microsoft.EntityFrameworkCore;


namespace Rota_LivreWEB_API.Repositories
{
    public class UsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CadastrarAsync(Usuario novoUsuario)
        {
            try
            {
                novoUsuario.senha = UsuarioDbContext.GerarHash(novoUsuario.senha);
                novoUsuario.resposta_seg = UsuarioDbContext.GerarHash(novoUsuario.resposta_seg);

                _context.Usuario.Add(novoUsuario);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar usuário: {ex.Message}");
                return false;
            }
        }

        public bool EmailExiste(string email)
        {
            return _context.Usuario.Any(u => u.email == email);
        }

        public Usuario? BuscarPorEmail(string email)
        {
            return _context.Usuario.FirstOrDefault(u => u.email == email);
        }

        public bool VerificarLogin(string email, string senha)
        {
            string senhaHash = UsuarioDbContext.GerarHash(senha);
            return _context.Usuario.Any(u => u.email == email && u.senha == senhaHash);
        }

        public int BuscarIdPorEmail(string email)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.email == email);
            return usuario?.id_usuario ?? 0;
        }

        public string BuscarNomePorEmail(string email)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.email == email);
            return usuario?.nome_completo ?? string.Empty;
        }

        public bool AlterarSenha(int idUsuario, string novaSenha)
        {
            var usuario = _context.Usuario.Find(idUsuario);
            if (usuario == null) return false;

            usuario.senha = UsuarioDbContext.GerarHash(novaSenha);
            _context.Usuario.Update(usuario);
            _context.SaveChanges();
            return true;
        }

        public Usuario? BuscarUsuarioPorEmail(string email)
        {
            return _context.Usuario.FirstOrDefault(u => u.email == email);

        }

        public string BuscarPerguntaDoUsuario(string email)
        {
            var usuario = _context.Usuario
                .Include(u => u.PerguntaSeguranca) 
                .FirstOrDefault(u => u.email == email);

            return usuario?.PerguntaSeguranca?.pergunta_seg ?? string.Empty;
        }

        public string ObterRespostaDoBanco(string email)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.email == email);
            return usuario?.resposta_seg ?? string.Empty;
        }


        public async Task AtualizarUsuarioAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public List<PerguntaSeguranca> ObterPerguntasDeSeguranca()
        {
            return _context.PerguntaSeguranca.ToList();
        }

        public async Task<Usuario?> BuscarPorIdAsync(int id)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.id_usuario == id);
        }




    }
}
    