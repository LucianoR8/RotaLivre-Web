using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Interfaces;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly IHomeService _service;

        public HomeApiController(IHomeService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Get(int usuarioId)
        {
            var home = await _service.GetHomeAsync(usuarioId);
            return Ok(home);
        }
    }
}