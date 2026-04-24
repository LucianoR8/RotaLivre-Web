using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

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

            var grupo = new Grupo
            {
                codigo_convite = codigo,
                id_passeio = passeioId,
                status = "CRIADO"
            };

            _context.Grupo.Add(grupo);
            await _context.SaveChangesAsync();

            return Ok(grupo);
        }

    }
}