using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Data; // Adicionado para acessar o banco
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Rota_LivreWEB_API.Controllers.Api
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PasseiosApiController : ControllerBase
    {
        private readonly IPasseioService _service;
        private readonly PasseioRepository _repo;
        private readonly AppDbContext _context; // Injetado para os métodos de Admin
        private readonly IConfiguration _config;

        public PasseiosApiController(IPasseioService service, PasseioRepository repo, AppDbContext context, IConfiguration config)
        {
            _service = service;
            _repo = repo;
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var passeios = await _service.GetAllAsync();
            return Ok(passeios);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var passeio = await _service.GetByIdComUsuarioAsync(id, int.Parse(userId));

            if (passeio == null)
                return NotFound();

            return Ok(passeio);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CriarPasseioDto dto)
        {
            var passeio = new Rota_LivreWEB_API.Models.Passeio
            {
                id_categoria = dto.CategoriaId,
                nome_passeio = dto.Nome,
                descricao = dto.Descricao,
                funcionamento = dto.Funcionamento,
                img_url = dto.ImagemUrl,
                status = "ativo"
            };

            _context.Passeio.Add(passeio);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = passeio.id_passeio },
                passeio
            );
        }

        [Authorize]
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult> GetByCategoria(int categoriaId)
        {
            var passeios = await _service.GetByCategoriaAsync(categoriaId);
            return Ok(passeios);
        }

        [Authorize]
        [HttpPost("{id}/curtir")]
        public async Task<ActionResult> Curtir(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var (curtiu, totalCurtidas) =
                await _service.AlternarCurtidaComTotalAsync(int.Parse(userId), id);

            return Ok(new
            {
                curtiu,
                totalCurtidas
            });
        }

        [Authorize]
        [HttpPost("{id}/pendente")]
        public async Task<ActionResult> AlternarPendente(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var pendente = await _service.AlternarPendenteAsync(int.Parse(userId), id);

            return Ok(new { pendente });
        }

        [Authorize]
        [HttpGet("meus")]
        public async Task<ActionResult> MeusPasseios()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var (curtidos, pendentes) =
                await _service.GetMeusPasseiosAsync(int.Parse(userId));

            return Ok(new
            {
                curtidos,
                pendentes
            });
        }

        [HttpGet("buscar")]
        public async Task<ActionResult> Buscar([FromQuery] string termo)
        {
            var passeios = await _repo.BuscarPasseioPorNomeAsync(termo);

            var resultado = passeios.Select(p => new PasseioDto
            {
                Id = p.id_passeio,
                Nome = p.nome_passeio,
                Descricao = p.descricao,
                ImagemUrl = $"{Request.Scheme}://{Request.Host}/img/passeios/{p.img_url}",
                QuantidadeCurtidas = p.QuantidadeCurtidas
            });

            return Ok(resultado);
        }

        // =========================================================================
        // MÉTODOS ADMINISTRATIVOS (CRUD COMPLETO DO REACT)
        // =========================================================================

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPasseio(
        int id,
        [FromBody] AtualizarPasseioDto dto)
            {
                var passeio = await _context.Passeio.FindAsync(id);

                if (passeio == null)
                {
                    return NotFound(new
                    {
                        mensagem = "Passeio não encontrado."
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                passeio.nome_passeio = dto.Nome;
                passeio.id_categoria = dto.CategoriaId;
                passeio.descricao = dto.Descricao;
                passeio.funcionamento = dto.Funcionamento;
                passeio.img_url = dto.ImagemUrl;
                passeio.status = string.IsNullOrWhiteSpace(dto.Status)
                    ? "ativo"
                    : dto.Status;

                passeio.atualizado_por =
                    userId != null
                        ? int.Parse(userId)
                        : null;

                passeio.atualizado_em = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(passeio);
            }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPasseio(int id)
        {
            var passeio = await _context.Passeio
                .FindAsync(id);

            if (passeio == null)
            {
                return NotFound(new
                {
                    mensagem = "Passeio não encontrado."
                });
            }

            _context.Passeio.Remove(passeio);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Passeio excluído com sucesso."
            });
        }

        [Authorize]
        [HttpPost("upload-imagem")]
        public async Task<ActionResult> UploadImagem(IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
            {
                return BadRequest("Imagem inválida.");
            }

            var tiposPermitidos = new[]
            {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

            if (!tiposPermitidos.Contains(imagem.ContentType.ToLower()))
            {
                return StatusCode(
                    415,
                    "Formato de imagem não suportado. Use JPG, PNG ou WEBP."
                );
            }

            var supabaseUrl = _config["Supabase:Url"];
            var supabaseKey = _config["Supabase:Key"];
            var bucket = _config["Supabase:Bucket"];

            if (string.IsNullOrWhiteSpace(supabaseUrl) ||
                string.IsNullOrWhiteSpace(supabaseKey) ||
                string.IsNullOrWhiteSpace(bucket))
            {
                return StatusCode(
                    500,
                    "Configuração do Supabase não encontrada."
                );
            }

            var extensao = Path.GetExtension(imagem.FileName)
                .ToLowerInvariant();

            if (string.IsNullOrEmpty(extensao))
            {
                return BadRequest(
                    "Não foi possível identificar a extensão da imagem."
                );
            }

            var fileName =
                $"fotos-passeios/passeio_{Guid.NewGuid()}{extensao}";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", supabaseKey);

            httpClient.DefaultRequestHeaders.Add(
                "apikey",
                supabaseKey
            );

            using var stream = imagem.OpenReadStream();

            using var content = new StreamContent(stream);

            content.Headers.ContentType =
                new MediaTypeHeaderValue(imagem.ContentType);

            var response = await httpClient.PostAsync(
                $"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();

                return BadRequest(erro);
            }

            var urlPublica =
                $"{supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";

            return Ok(new
            {
                imagemUrl = urlPublica
            });
        }
    }
}