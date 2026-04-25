using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Interfaces;
using Rota_LivreWEB_API.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var passeio = await _service.GetByIdAsync(id);

                if (passeio == null)
                    return NotFound();

                return Ok(passeio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString()); 
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PasseioDto dto)
        {
            var novo = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = novo.Id }, novo);
        }
    }
}