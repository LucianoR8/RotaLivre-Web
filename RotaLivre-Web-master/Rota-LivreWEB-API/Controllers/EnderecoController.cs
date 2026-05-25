using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly EnderecoRepository _enderecoRepository;

        public EnderecoController(EnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        // GET api/endereco/passeio/5
        [HttpGet("passeio/{idPasseio}")]
        public async Task<IActionResult> GetByPasseio(int idPasseio)
        {
            var endereco = await _enderecoRepository.BuscarPorPasseioAsync(idPasseio);

            if (endereco == null)
                return NotFound();

            return Ok(new
            {
                endereco.Latitude,
                endereco.Longitude,
                endereco.RaioMetros
            });
        }
    }
}