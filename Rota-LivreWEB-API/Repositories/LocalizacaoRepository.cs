using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Data;
using Microsoft.EntityFrameworkCore;

namespace Rota_LivreWEB_API.Repositories
{
    public class LocalizacaoRepository
    {
        private readonly AppDbContext _context;

        public LocalizacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SalvarHistoricoAsync(Localizacao novaLocalizacao, int idUsuario)
        {
            try
            {
                // gera id_localizacao'
                _context.Localizacao.Add(novaLocalizacao);
                await _context.SaveChangesAsync();

                var vinculo = new UsuarioLocalizacao
                {
                    id_usuario = idUsuario,
                    id_localizacao = novaLocalizacao.id_localizacao
                };

                _context.UsuarioLocalizacao.Add(vinculo);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar localização: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Localizacao>> ObterHistoricoPorUsuarioAsync(int idUsuario)
        {
            return await _context.UsuarioLocalizacao
                .Where(ul => ul.id_usuario == idUsuario)
                .Include(ul => ul.Localizacao)
                .Select(ul => ul.Localizacao!)
                .OrderByDescending(l => l.data_registro)
                .ToListAsync();
        }
    }
}