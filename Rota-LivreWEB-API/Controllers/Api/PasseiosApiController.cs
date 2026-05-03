using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rota_LivreWEB_API.Controllers.Api
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PasseiosApiController : ControllerBase
    {
        private readonly IPasseioService _service;

        public PasseiosApiController(IPasseioService service)
        {
            _service = service;
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
    }
}