using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Repositories
{
    public class EnderecoRepository
    {
        private readonly AppDbContext _context;

        public EnderecoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Endereco?> BuscarPorPasseioAsync(int idPasseio)
        {
            return await _context.Endereco
                .FirstOrDefaultAsync(e => e.id_passeio == idPasseio);
        }
    }
}