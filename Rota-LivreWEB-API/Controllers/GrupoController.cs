using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("api/grupo")]
    public class GrupoController : ControllerBase
    {
        private readonly AppDbContext _context;

        [HttpGet("/grupo")]
        public ActionResult AbrirGrupo(string codigo)
        {
            var deepLink = $"rotalivre://grupo?codigo={codigo}";

            var html = $@"
    <html>
    <head>
        <script>
            function abrirApp() {{
                window.location.href = '{deepLink}';
            }}

            setTimeout(abrirApp, 500);
        </script>
    </head>
    <body>
        <h2>Abrindo o app...</h2>

        <p>Se não abrir automaticamente:</p>
        <button onclick='abrirApp()'>Abrir no app</button>

        <p>Código do grupo:</p>
        <h3>{codigo}</h3>
    </body>
    </html>";

            return Content(html, "text/html");
        }


        public GrupoController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("criar")]
        public async Task<ActionResult> CriarGrupo(int passeioId)
        {
            try
            {
                var codigo = Guid.NewGuid().ToString().Substring(0, 6);

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                    return Unauthorized("Usuário não autenticado");

                var userId = int.Parse(userIdClaim);

                var grupo = new Grupo
                {
                    codigo_convite = codigo,
                    id_passeio = passeioId,
                    id_criador = userId,
                    nome = "Grupo do passeio",
                    status = "CRIADO"
                };

                _context.Grupo.Add(grupo);
                await _context.SaveChangesAsync();

                return Ok(grupo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("validar")]
        public async Task<ActionResult<bool>> ValidarGrupo(string codigo)
        {
            var grupo = await _context.Grupo
                .FirstOrDefaultAsync(g => g.codigo_convite == codigo);

            if (grupo == null)
                return false;

            return grupo.status != "FINALIZADO";
        }

    }
}