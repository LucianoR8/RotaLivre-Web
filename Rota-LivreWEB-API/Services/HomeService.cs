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
            // CATEGORIAS (DEDUÇÃO INTELIGENTE ATRAVÉS DOS PASSEIOS)
            // =========================================================

            // 1. Buscamos todas as categorias ativas
            var categoriasDb = await _context.Categoria
                .Where(c => c.ativo)
                .ToListAsync();

            // 2. Buscamos TODOS os passeios ativos para deduzir as ligações
            // Trazemos apenas id_cidade e id_categoria para a consulta ser ultra rápida
            var passeiosDb = await _context.Passeio
                .Where(p => p.status == "ativo" && p.id_cidade != null && p.id_categoria != null)
                .Select(p => new { p.id_cidade, p.id_categoria })
                .ToListAsync();

            // 3. Mapeamos as categorias e o C# deduz os vínculos automaticamente!
            var categorias = categoriasDb.Select(c => new CategoriaDto
            {
                IdCategoria = c.id_categoria,
                TipoCategoria = c.tipo_categoria,
                ImgUrl = c.img,
                Ativo = c.ativo,
                Classificacao = string.IsNullOrWhiteSpace(c.classificacao) ? "TEMA" : c.classificacao,

                // A MÁGICA ACONTECE AQUI:
                // Se a categoria for um TEMA, procura nos passeios todas as cidades 
                // que têm passeios vinculados a este tema. Pega apenas os IDs únicos (Distinct).
                CidadesVinculadas = (string.IsNullOrWhiteSpace(c.classificacao) ? "TEMA" : c.classificacao) == "TEMA"
                    ? passeiosDb
                        .Where(p => p.id_categoria == c.id_categoria && p.id_cidade.HasValue)
                        .Select(p => p.id_cidade.Value)
                        .Distinct()
                        .ToList()
                    : new List<int>()
            })
            .ToList();

            // =========================================================
            // DESTAQUES
            // =========================================================

            var destaques = await _context.Passeio
                .Where(p => p.status == "ativo")
                .Select(p => new PasseioDto
                {
                    Id = p.id_passeio,
                    Nome = p.nome_passeio,
                    Descricao = p.descricao,
                    Funcionamento = p.funcionamento,
                    ImagemUrl = p.img_url,

                    CategoriaId = p.id_categoria,
                    CidadeId = p.id_cidade,

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
                    CidadeId = c.Passeio.id_cidade,

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