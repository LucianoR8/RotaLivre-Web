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
                    setTimeout(function() {{
                        window.location.href = 'https://play.google.com/store';
                    }}, 2000);
                </script>
            </head>
            <body>
                <p>Abrindo aplicativo...</p>
            </body>
            </html>";

            return Content(html, "text/html");
        }
    }
}