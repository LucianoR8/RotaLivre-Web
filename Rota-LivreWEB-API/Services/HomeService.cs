using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;

namespace Rota_LivreWEB_API.Services
{
    public class HomeService : IHomeService
    {
        private readonly AppDbContext _context;

        public HomeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HomeDto> GetHomeAsync(int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId);

            var destaques = await _context.Passeio
                .OrderByDescending(p => p.QuantidadeCurtidas)
                .Take(5)
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = p.img_url,
                    QuantidadeCurtidas = p.QuantidadeCurtidas
                })
                .ToListAsync();

            return new HomeDto
            {
                NomeUsuario = usuario?.nome_completo,
                Destaques = destaques
            };
        }
    }
}