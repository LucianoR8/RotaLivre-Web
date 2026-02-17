using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.DTOs;
using System.Threading.Tasks;

namespace Rota_LivreWEB_API.Controllers.Api
{
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
        public async Task<IActionResult> Get()
        {
            var passeios = await _service.GetAllAsync();
            return Ok(passeios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var passeio = await _service.GetByIdAsync(id);

            if (passeio == null)
                return NotFound();

            return Ok(passeio);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PasseioDto dto)
        {
            var novo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = novo.Id }, novo);
        }
    }
}