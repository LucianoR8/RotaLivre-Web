using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories; 


namespace Rota_LivreWEB_API.Repositories
{
    public class PasseioRepository
    {
        private readonly AppDbContext _context;

        public PasseioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Passeio>> BuscarPasseiosPorCategoriaAsync(int idCategoria)
        {
            return await _context.Passeio
                .Where(p => p.id_categoria == idCategoria)
                .ToListAsync();
        }

        public async Task<Passeio?> BuscarPasseioPorIdAsync(int id)
        {
            return await _context.Passeio
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.id_passeio == id);
        }

        public async Task<List<Categoria>> BuscarCategoriasAsync()
        {
            return await _context.Categoria.ToListAsync();
        }

        public async Task InserirAvaliacaoAsync(int idPasseio, int idUsuario, int nota, string feedback)
        {
            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var avaliacao = new Avaliacao
            {
                id_passeio = idPasseio,
                id_usuario = idUsuario,
                nota = nota,
                feedback = feedback,
                data_feedback = DateTime.UtcNow,
                nome_completo = usuario.nome_completo,
                Usuario = usuario
            };

            await _context.Avaliacao.AddAsync(avaliacao);
            await _context.SaveChangesAsync();
        }



        public async Task<List<Avaliacao>> ListarAvaliacoesPorPasseioAsync(int idPasseio)
        {
            return await _context.Avaliacao
                .Include(a => a.Usuario)
                .Where(a => a.id_passeio == idPasseio)
                .OrderByDescending(a => a.data_feedback)
                .ToListAsync();
        }

        public async Task<List<Passeio>> BuscarPasseiosCurtidosPorUsuarioAsync(int idUsuario)
        {
            return await _context.CurtidaPasseio
                .Where(c => c.id_usuario == idUsuario)
                .Include(c => c.Passeio)
                .Select(c => c.Passeio)
                .ToListAsync();
        }

        public async Task<List<Passeio>> BuscarPasseiosPendentesPorUsuarioAsync(int idUsuario)
        {
            return await _context.PasseioPendente
                .Where(pp => pp.id_usuario == idUsuario)
                .Include(pp => pp.Passeio)
                .Select(pp => pp.Passeio)
                .ToListAsync();
        }

        public async Task AdicionarPasseioPendenteAsync(int idUsuario, int idPasseio)
        {
            var existe = await _context.PasseioPendente
                .AnyAsync(pp => pp.id_usuario == idUsuario && pp.id_passeio == idPasseio);

            if (!existe)
            {
                var novo = new PasseioPendente
                {
                    id_usuario = idUsuario,
                    id_passeio = idPasseio,
                    data_adicao = DateTime.UtcNow
                };

                await _context.PasseioPendente.AddAsync(novo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> VerificarPasseioPendenteAsync(int idUsuario, int idPasseio)
        {
            return await _context.PasseioPendente
                .AnyAsync(pp => pp.id_usuario == idUsuario && pp.id_passeio == idPasseio);
        }

        public async Task RemoverPasseioPendenteAsync(int idUsuario, int idPasseio)
        {
            var item = await _context.PasseioPendente
                .FirstOrDefaultAsync(pp => pp.id_usuario == idUsuario && pp.id_passeio == idPasseio);

            if (item != null)
            {
                _context.PasseioPendente.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PasseioExisteAsync(int idPasseio)
        {
            return await _context.Passeio.AnyAsync(p => p.id_passeio == idPasseio);
        }

        public async Task<int> ObterTotalCurtidasAsync(int idPasseio)
        {
            return await _context.CurtidaPasseio.CountAsync(c => c.id_passeio == idPasseio);
        }

        public async Task<bool> UsuarioJaCurtiuAsync(int idUsuario, int idPasseio)
        {
            return await _context.CurtidaPasseio
                .AnyAsync(c => c.id_usuario == idUsuario && c.id_passeio == idPasseio);
        }

        public async Task<bool> AlternarCurtidaAsync(int idUsuario, int idPasseio)
        {
            var curtida = await _context.CurtidaPasseio
                .FirstOrDefaultAsync(c => c.id_usuario == idUsuario && c.id_passeio == idPasseio);

            if (curtida != null)
            {
                _context.CurtidaPasseio.Remove(curtida);
                await _context.SaveChangesAsync();
                return false; 
            }

            var novaCurtida = new CurtidaPasseio
            {
                id_usuario = idUsuario,
                id_passeio = idPasseio
            };

            await _context.CurtidaPasseio.AddAsync(novaCurtida);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<List<Passeio>> BuscarPasseioPorNomeAsync(string termo)
        {
            var lista = await _context.Passeio
                .Where(p => p.nome_passeio.Contains(termo))
                .ToListAsync();

            foreach (var p in lista)
            {
                p.QuantidadeCurtidas = await _context.CurtidaPasseio.CountAsync(c => c.id_passeio == p.id_passeio);
            }

            return lista;
        }

        public async Task<List<Passeio>> BuscarPasseiosMaisCurtidosAsync()
        {
            return await _context.Passeio
                .Select(p => new Passeio
                {
                    id_passeio = p.id_passeio,
                    nome_passeio = p.nome_passeio,
                    descricao = p.descricao,
                    img_url = p.img_url,
                    QuantidadeCurtidas = _context.CurtidaPasseio.Count(c => c.id_passeio == p.id_passeio)
                })
                .OrderByDescending(p => p.QuantidadeCurtidas)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Passeio>> ObterTodosAsync()
        {
            return await _context.Passeio.ToListAsync();
        }


    }
}
