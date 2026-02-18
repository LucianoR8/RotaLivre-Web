using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Rota_LivreWEB_API.Interfaces;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly IHomeService _service;

        public HomeApiController(IHomeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var home = await _service.GetHomeAsync(int.Parse(userId));

            return Ok(home);
        }
    }
}