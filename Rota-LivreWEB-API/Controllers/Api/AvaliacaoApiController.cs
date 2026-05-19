using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvaliacaoApiController : ControllerBase
    {
        private readonly PasseioRepository _repo;

        public AvaliacaoApiController(PasseioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{idPasseio}")]
        public async Task<ActionResult> Listar(int idPasseio)
        {
            var lista =
                await _repo.ListarAvaliacoesPorPasseioAsync(idPasseio);

            var resultado = lista.Select(a => new AvaliacaoDto
            {
                NomeUsuario = a.nome_completo,
                Feedback = a.feedback,
                Data = a.data_feedback
            });

            return Ok(resultado);
        }

        [HttpPost("comentar")]
        public async Task<ActionResult> Comentar(
            [FromBody] CriarAvaliacaoDto dto)
        {
            await _repo.InserirAvaliacaoAsync(
                dto.IdPasseio,
                dto.IdUsuario,
                5,
                dto.Feedback);

            return Ok();
        }
    }
}