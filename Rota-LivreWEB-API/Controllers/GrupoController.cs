using Microsoft.AspNetCore.Mvc;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("")]
    public class GrupoController : Controller
    {
        [HttpGet("grupo")]
        public IActionResult AbrirGrupo(string codigo)
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
    }
}