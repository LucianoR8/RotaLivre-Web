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

        public PasseiosApiController(IPasseioService service, PasseioRepository repo, AppDbContext context)
        {
            _service = service;
            _repo = repo;
            _context = context;
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

        // POST ORIGINAL (Mantido para compatibilidade com o app atual)
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PasseioDto dto)
        {
            var novo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = novo.Id }, novo);
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

        [Authorize] // O ideal é proteger para que só logados mexam
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarPasseio(int id, [FromBody] Rota_LivreWEB_API.Models.Passeio passeioAtualizado)
        {
            var passeio = await _context.Passeio.FindAsync(id);
            if (passeio == null) return NotFound(new { mensagem = "Passeio não encontrado." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            passeio.nome_passeio = passeioAtualizado.nome_passeio;
            passeio.id_categoria = passeioAtualizado.id_categoria;
            passeio.descricao = passeioAtualizado.descricao;
            passeio.funcionamento = passeioAtualizado.funcionamento;
            passeio.img_url = passeioAtualizado.img_url;
            passeio.status = passeioAtualizado.status ?? "ativo";

            passeio.atualizado_por = userId != null ? int.Parse(userId) : null;
            passeio.atualizado_em = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(passeio);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarPasseio(int id)
        {
            var passeio = await _context.Passeio.FindAsync(id);
            if (passeio == null) return NotFound(new { mensagem = "Passeio não encontrado." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Soft delete: Apenas desativa para não quebrar tabelas dependentes (avaliações, etc)
            passeio.status = "inativo";
            passeio.atualizado_por = userId != null ? int.Parse(userId) : null;
            passeio.atualizado_em = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Passeio desativado com sucesso." });
        }
    }
}