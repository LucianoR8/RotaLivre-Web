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
            // CATEGORIAS (CRUZAMENTO SEGURO EM MEMÓRIA)
            // =========================================================

            // 1. Buscamos todas as categorias ativas e guardamos na memória
            var categoriasDb = await _context.Categoria
                .Where(c => c.ativo)
                .ToListAsync();

            // 2. Buscamos TODOS os vínculos diretamente da tabela intermediária
            var vinculosDb = await _context.CategoriaVinculo.ToListAsync();

            // 3. Mapeamos para o DTO e cruzamos os dados manualmente. 
            // Como fazemos isto em memória, o Entity Framework não consegue ignorar os vínculos.
            var categorias = categoriasDb.Select(c => new CategoriaDto
            {
                IdCategoria = c.id_categoria,
                TipoCategoria = c.tipo_categoria,
                ImgUrl = c.img,
                Ativo = c.ativo,
                Classificacao = string.IsNullOrWhiteSpace(c.classificacao) ? "TEMA" : c.classificacao,

                // Onde o id do tema no vínculo for igual ao id desta categoria, extrai o id da cidade
                CidadesVinculadas = vinculosDb
                    .Where(v => v.id_tema == c.id_categoria)
                    .Select(v => v.id_cidade)
                    .ToList()
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