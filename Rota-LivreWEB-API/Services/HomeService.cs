using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.Models;

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
            // =========================================================
            // USUÁRIO
            // =========================================================

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(
                    u => u.id_usuario == usuarioId
                );

            // =========================================================
            // CATEGORIAS (AGORA COM CIDADES E TEMAS - SEGURO)
            // =========================================================

            // 1. Trazemos os dados para a memória (Garante que o EF Core não vai ignorar os vínculos)
            var categoriasDb = await _context.Categoria
                .Where(c => c.ativo)
                .Include(c => c.VinculosComoTema)
                .ToListAsync();

            // 2. Fazemos o mapeamento já com os dados seguros na memória
            var categorias = categoriasDb.Select(c => new CategoriaDto
            {
                IdCategoria = c.id_categoria,
                TipoCategoria = c.tipo_categoria,
                ImgUrl = c.img,

                // Trata a classificação para nunca ir nula
                Classificacao = string.IsNullOrWhiteSpace(c.classificacao) ? "TEMA" : c.classificacao,

                // Mapeia as cidades com 100% de garantia
                CidadesVinculadas = c.VinculosComoTema.Select(v => v.id_cidade).ToList()
            })
            .ToList();

            // =========================================================
            // DESTAQUES
            // =========================================================

            var destaques = await _context.Passeio
                .Where(p => p.status == "ativo") // Garante que passeios inativos não apareçam
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = p.img_url,

                    CategoriaId = p.id_categoria,
                    CidadeId = p.id_cidade, // Enviando a cidade para o Front

                    QuantidadeCurtidas = _context.CurtidaPasseio
                            .Count(c => c.id_passeio == p.id_passeio)
                })
                .OrderByDescending(p => p.QuantidadeCurtidas)
                .Take(5)
                .ToListAsync();

            // =========================================================
            // FAVORITADOS
            // =========================================================

            var favoritados = await _context.CurtidaPasseio
                .Where(c => c.id_usuario == usuarioId && c.Passeio.status == "ativo")
                .Select(c => new PasseioDto
                {
                    Id = c.Passeio.id_passeio,
                    Nome = c.Passeio.nome_passeio,
                    Descricao = c.Passeio.descricao,
                    Funcionamento = c.Passeio.funcionamento,
                    ImagemUrl = c.Passeio.img_url,

                    CategoriaId = c.Passeio.id_categoria,
                    CidadeId = c.Passeio.id_cidade, // Enviando a cidade para o Front

                    QuantidadeCurtidas = _context.CurtidaPasseio
                            .Count(cp => cp.id_passeio == c.id_passeio),

                    UsuarioJaCurtiu = true
                })
                .ToListAsync();

            // =========================================================
            // RETORNO
            // =========================================================

            return new HomeDto
            {
                NomeUsuario = usuario?.nome_completo,
                Destaques = destaques,
                Categorias = categorias,
                Favoritados = favoritados
            };
        }
    }
}