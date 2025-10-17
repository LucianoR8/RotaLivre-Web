using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Repositories
{
    public class CategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> ObterCategoriasAsync()
        {
            return await _context.Categoria.ToListAsync();
        }

        public async Task<Categoria?> ObterCategoriaPorIdAsync(int id)
        {
            return await _context.Categoria
                .FirstOrDefaultAsync(c => c.id_categoria == id);
        }

        public async Task AdicionarCategoriaAsync(Categoria categoria)
        {
            await _context.Categoria.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarCategoriaAsync(Categoria categoria)
        {
            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverCategoriaAsync(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria != null)
            {
                _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }
    }
}
