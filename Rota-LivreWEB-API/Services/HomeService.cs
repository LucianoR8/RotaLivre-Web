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
            // CATEGORIAS (TRUQUE DO TIPO ANÔNIMO PARA FORÇAR O JOIN)
            // =========================================================

            // 1. O EF Core funciona na perfeição quando projetamos para tipos anônimos
            var categoriasDb = await _context.Categoria
                .Where(c => c.ativo)
                .Select(c => new
                {
                    c.id_categoria,
                    c.tipo_categoria,
                    c.img,
                    c.classificacao,
                    c.ativo,
                    // Aqui o EF Core não tem como falhar
                    IdsCidades = c.VinculosComoTema.Select(v => v.id_cidade).ToList()
                })
                .ToListAsync();

            // 2. Agora mapeamos para o DTO na memória do servidor
            var categorias = categoriasDb.Select(c => new CategoriaDto
            {
                IdCategoria = c.id_categoria,
                TipoCategoria = c.tipo_categoria,
                ImgUrl = c.img,
                Ativo = c.ativo,
                Classificacao = string.IsNullOrWhiteSpace(c.classificacao) ? "TEMA" : c.classificacao,
                CidadesVinculadas = c.IdsCidades
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