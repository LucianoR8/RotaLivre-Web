using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using System.Net.Http.Headers;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public CategoriaApiController(
            AppDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =========================================================
        // LISTAR CATEGORIAS
        // =========================================================

        // =========================================================
        // LISTAR CATEGORIAS
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categoria
                .Where(c => c.ativo)
                .Select(c => new
                {
                    id = c.id_categoria,
                    tipo_categoria = c.tipo_categoria,
                    img = c.img,
                    ativo = c.ativo,
                    classificacao = c.classificacao, // Retorna se é CIDADE ou TEMA
                    atualizado_por = c.atualizado_por,
                    atualizado_em = c.atualizado_em,

                    // Traz a lista de IDs das cidades que este tema pertence (para marcar as checkboxes no Front)
                    cidadesVinculadas = c.VinculosComoTema.Select(v => v.id_cidade).ToList(),

                    tourCount = _context.Passeio.Count(p => p.id_categoria == c.id_categoria)
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // =========================================================
        // CRIAR CATEGORIA
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CriarCategoria([FromBody] CategoriaDto dto)
        {
            var categoria = new Categoria
            {
                tipo_categoria = dto.TipoCategoria,
                img = dto.ImgUrl,
                ativo = dto.Ativo,
                classificacao = string.IsNullOrWhiteSpace(dto.Classificacao) ? "TEMA" : dto.Classificacao,
                atualizado_em = DateTime.UtcNow
            };

            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync(); // Gera o ID da categoria nova

            // Se for um TEMA e vieram cidades vinculadas, salva na tabela intermediária
            if (categoria.classificacao == "TEMA" && dto.CidadesVinculadas != null && dto.CidadesVinculadas.Any())
            {
                foreach (var idCidade in dto.CidadesVinculadas)
                {
                    _context.CategoriaVinculo.Add(new CategoriaVinculo
                    {
                        id_tema = categoria.id_categoria,
                        id_cidade = idCidade
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.id_categoria }, categoria);
        }

        // =========================================================
        // ATUALIZAR CATEGORIA
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCategoria(int id, [FromBody] CategoriaDto dto)
        {
            var categoria = await _context.Categoria
                .Include(c => c.VinculosComoTema)
                .FirstOrDefaultAsync(c => c.id_categoria == id);

            if (categoria == null)
                return NotFound();

            categoria.tipo_categoria = dto.TipoCategoria;
            categoria.img = dto.ImgUrl;
            categoria.ativo = dto.Ativo;
            categoria.classificacao = string.IsNullOrWhiteSpace(dto.Classificacao) ? "TEMA" : dto.Classificacao;
            categoria.atualizado_em = DateTime.UtcNow;

            // Atualiza os vínculos se for um TEMA
            if (categoria.classificacao == "TEMA")
            {
                // 1. Remove os vínculos antigos
                if (categoria.VinculosComoTema.Any())
                {
                    _context.CategoriaVinculo.RemoveRange(categoria.VinculosComoTema);
                }

                // 2. Adiciona os novos vínculos selecionados no Front-end
                if (dto.CidadesVinculadas != null && dto.CidadesVinculadas.Any())
                {
                    foreach (var idCidade in dto.CidadesVinculadas)
                    {
                        _context.CategoriaVinculo.Add(new CategoriaVinculo
                        {
                            id_tema = categoria.id_categoria,
                            id_cidade = idCidade
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        // =========================================================
        // DELETAR CATEGORIA
        // =========================================================

        // =========================================================
        // DELETAR CATEGORIA
        // =========================================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarCategoria(int id)
        {
            var categoria =
                await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensagem = "Categoria não encontrada."
                });
            }

            // Verifica se existem passeios usando esta categoria
            var possuiPasseios =
                await _context.Passeio
                    .AnyAsync(p => p.id_categoria == id);

            if (possuiPasseios)
            {
                return Conflict(new
                {
                    mensagem =
                        "Não é possível excluir esta categoria porque existem passeios vinculados a ela."
                });
            }

            // Exclusão REAL da categoria
            _context.Categoria.Remove(categoria);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Categoria excluída definitivamente."
            });
        }

        // =========================================================
        // UPLOAD DA IMAGEM DA CATEGORIA
        // =========================================================

        [Authorize]
        [HttpPost("upload-imagem")]
        public async Task<ActionResult> UploadImagem(
            IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
            {
                return BadRequest("Imagem inválida.");
            }

            // -----------------------------------------------------
            // FORMATOS PERMITIDOS
            // -----------------------------------------------------

            var tiposPermitidos = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!tiposPermitidos.Contains(
                    imagem.ContentType.ToLower()))
            {
                return StatusCode(
                    415,
                    "Formato de imagem não suportado. Use JPG, PNG ou WEBP."
                );
            }

            // -----------------------------------------------------
            // CONFIGURAÇÃO DO SUPABASE
            // -----------------------------------------------------

            var supabaseUrl =
                _config["Supabase:Url"];

            var supabaseKey =
                _config["Supabase:Key"];

            var bucket =
                _config["Supabase:Bucket"];

            if (string.IsNullOrWhiteSpace(supabaseUrl) ||
                string.IsNullOrWhiteSpace(supabaseKey) ||
                string.IsNullOrWhiteSpace(bucket))
            {
                return StatusCode(
                    500,
                    "Configuração do Supabase não encontrada."
                );
            }

            // -----------------------------------------------------
            // EXTENSÃO
            // -----------------------------------------------------

            var extensao =
                Path.GetExtension(imagem.FileName)
                    .ToLowerInvariant();

            if (string.IsNullOrEmpty(extensao))
            {
                return BadRequest(
                    "Não foi possível identificar a extensão da imagem."
                );
            }

            // -----------------------------------------------------
            // NOME DO ARQUIVO
            // -----------------------------------------------------

            var fileName =
                $"fotos-categorias/categoria_{Guid.NewGuid()}{extensao}";

            // -----------------------------------------------------
            // ENVIO PARA SUPABASE STORAGE
            // -----------------------------------------------------

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    supabaseKey
                );

            httpClient.DefaultRequestHeaders.Add(
                "apikey",
                supabaseKey
            );

            using var stream =
                imagem.OpenReadStream();

            using var content =
                new StreamContent(stream);

            content.Headers.ContentType =
                new MediaTypeHeaderValue(
                    imagem.ContentType
                );

            var response =
                await httpClient.PostAsync(
                    $"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}",
                    content
                );

            // -----------------------------------------------------
            // ERRO NO SUPABASE
            // -----------------------------------------------------

            if (!response.IsSuccessStatusCode)
            {
                var erro =
                    await response.Content.ReadAsStringAsync();

                return BadRequest(erro);
            }

            // -----------------------------------------------------
            // URL PÚBLICA
            // -----------------------------------------------------

            var urlPublica =
                $"{supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";

            return Ok(new
            {
                imagemUrl = urlPublica
            });
        }
    }
}