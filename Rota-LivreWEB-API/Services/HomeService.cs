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
            // CATEGORIAS
            // =========================================================
            //
            // ATENÇÃO:
            // Aqui mantemos temporariamente o Render porque
            // o campo de categoria ainda parece armazenar
            // somente o nome do arquivo.
            //
            // Quando migrarmos as categorias para o Supabase,
            // trocamos ImgUrl para c.img.
            //

            var categorias = await _context.Categoria
                .Select(c => new CategoriaDto
                {
                    IdCategoria =
                        c.id_categoria,

                    TipoCategoria =
                        c.tipo_categoria,

                    ImgUrl =
                        $"https://rotalivre-web.onrender.com/img/categorias/{c.img}"
                })
                .ToListAsync();


            // =========================================================
            // DESTAQUES
            // =========================================================
            //
            // Agora a imagem vem diretamente do banco.
            // O banco deve conter a URL pública do Supabase.
            //

            var destaques = await _context.Passeio
                .Select(p => new PasseioDto
                {
                    Id =
                        p.id_passeio,

                    Nome =
                        p.nome_passeio,

                    Descricao =
                        p.descricao,

                    Funcionamento =
                        p.funcionamento,

                    ImagemUrl =
                        p.img_url,

                    QuantidadeCurtidas =
                        _context.CurtidaPasseio
                            .Count(c =>
                                c.id_passeio ==
                                p.id_passeio)
                })
                .OrderByDescending(
                    p => p.QuantidadeCurtidas
                )
                .Take(5)
                .ToListAsync();


            // =========================================================
            // FAVORITADOS
            // =========================================================

            var favoritados =
                await _context.CurtidaPasseio

                    .Where(c =>
                        c.id_usuario ==
                        usuarioId)

                    .Select(c => new PasseioDto
                    {
                        Id =
                            c.Passeio.id_passeio,

                        Nome =
                            c.Passeio.nome_passeio,

                        Descricao =
                            c.Passeio.descricao,

                        Funcionamento =
                            c.Passeio.funcionamento,

                        // URL direta do Supabase
                        ImagemUrl =
                            c.Passeio.img_url,

                        QuantidadeCurtidas =
                            _context.CurtidaPasseio
                                .Count(cp =>
                                    cp.id_passeio ==
                                    c.id_passeio),

                        UsuarioJaCurtiu =
                            true
                    })

                    .ToListAsync();


            // =========================================================
            // RETORNO
            // =========================================================

            return new HomeDto
            {
                NomeUsuario =
                    usuario?.nome_completo,

                Destaques =
                    destaques,

                Categorias =
                    categorias,

                Favoritados =
                    favoritados
            };
        }
    }
}