using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using System.Security.Claims;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("api/grupo")]
    public class GrupoController : ControllerBase
    {
        private readonly AppDbContext _context;

        [HttpGet("grupo")]
        public ActionResult AbrirGrupo(string codigo)
        {
            var deepLink = $"rotalivre://grupo?codigo={codigo}";

            var html = $@"
                <html>
                <head>
                    <script>
                        window.location.href = '{deepLink}';
                    </script>
                </head>
                <body>
                    <h2>Abra o app Rota Livre</h2>
                    <p>Para entrar no grupo, é necessário o aplicativo instalado.</p>
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

        [HttpPost("criar")]
        public async Task<ActionResult> CriarGrupo(int passeioId)
        {
            var codigo = Guid.NewGuid().ToString().Substring(0, 6);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Usuário não autenticado");

            var userId = int.Parse(userIdClaim.Value);

            var grupo = new Grupo
            {
                codigo_convite = codigo,
                id_passeio = passeioId,
                id_criador = userId,
                status = "CRIADO"
            };

            _context.Grupo.Add(grupo);
            await _context.SaveChangesAsync();

            return Ok(grupo);
        }

    }
}