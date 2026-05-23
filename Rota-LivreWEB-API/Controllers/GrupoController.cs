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
            var deepLink = $"rotalivre://grupo/entrar?codigo={codigo}";

            var html = $@"
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1' />

    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background: #f5f7fb;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }}

        .card {{
            background: white;
            width: 90%;
            max-width: 420px;
            border-radius: 20px;
            padding: 35px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.08);
            text-align: center;
        }}

        .logo {{
            font-size: 52px;
            margin-bottom: 10px;
        }}

        h1 {{
            margin: 0;
            color: #222;
            font-size: 28px;
        }}

        p {{
            color: #666;
            font-size: 16px;
            margin-top: 10px;
        }}

        .codigo {{
            margin-top: 25px;
            background: #f1f4f9;
            border-radius: 14px;
            padding: 18px;
            font-size: 30px;
            font-weight: bold;
            letter-spacing: 3px;
            color: #1976d2;
        }}

        .botao {{
            display: inline-block;
            margin-top: 30px;
            background: #1976d2;
            color: white;
            padding: 16px 28px;
            border-radius: 14px;
            text-decoration: none;
            font-size: 18px;
            font-weight: bold;
            transition: 0.2s;
        }}

        .botao:active {{
            transform: scale(0.98);
        }}

        .rodape {{
            margin-top: 25px;
            font-size: 14px;
            color: #888;
        }}
    </style>
</head>

<body>

    <div class='card'>

        <div class='logo'>🚶</div>

        <h1>Rota Livre</h1>

        <p>Você foi convidado para entrar em um grupo de passeio.</p>

        <div class='codigo'>
            {codigo}
        </div>

        <a class='botao' href='{deepLink}'>
            ABRIR NO APP
        </a>

        <div class='rodape'>
            Se o aplicativo não abrir automaticamente,<br/>
            volte ao app que o código será detectado.
        </div>

    </div>

    <script>
        setTimeout(function(){{
            window.location.href = '{deepLink}';
        }}, 1200);
    </script>

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