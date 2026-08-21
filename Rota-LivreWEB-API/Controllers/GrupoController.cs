using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.DTOs.Grupo;
using Rota_LivreWEB_API.Models;
using System.Security.Claims;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("api/grupo")]
    public class GrupoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GrupoController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // ABRIR LINK DE CONVITE
        // =========================================================

        [HttpGet("/grupo")]
        public async Task<ActionResult> AbrirGrupo(string codigo)
        {
            var grupo = await _context.Grupo
                .FirstOrDefaultAsync(g => g.codigo_convite == codigo);

            if (grupo == null)
                return NotFound("Grupo não encontrado.");

            if (grupo.status == "FINALIZADO")
                return BadRequest("Este grupo já foi finalizado.");

            var deepLink =
                $"rotalivre://grupo/entrar?codigo={codigo}";

            var html = $@"
<html>
<head>
    <meta charset='UTF-8'>
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


        // =========================================================
        // CRIAR GRUPO
        // =========================================================

        [Authorize]
        [HttpPost("criar")]
        public async Task<ActionResult> CriarGrupo(
            [FromBody] CriarGrupoDto dto)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                    return Unauthorized("Usuário não autenticado.");

                var userId = int.Parse(userIdClaim);

                var passeio = await _context.Passeio
                    .FirstOrDefaultAsync(
                        p => p.id_passeio == dto.PasseioId);

                if (passeio == null)
                    return NotFound("Passeio não encontrado.");


                // -------------------------------------------------
                // GERA CÓDIGO DE CONVITE
                // -------------------------------------------------

                var codigo = Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 6)
                    .ToUpper();


                // -------------------------------------------------
                // CRIA GRUPO
                // -------------------------------------------------

                var grupo = new Grupo
                {
                    nome = string.IsNullOrWhiteSpace(dto.Nome)
                        ? "Grupo do passeio"
                        : dto.Nome,

                    codigo_convite = codigo,

                    id_criador = userId,

                    id_passeio = dto.PasseioId,

                    status = "CRIADO",

                    data_criacao = DateTime.UtcNow,

                    data_inicio = dto.DataInicio
                };

                _context.Grupo.Add(grupo);

                await _context.SaveChangesAsync();


                // -------------------------------------------------
                // ADICIONA CRIADOR AO GRUPO
                // -------------------------------------------------

                var grupoUsuario = new GrupoUsuario
                {
                    id_grupo = grupo.id_grupo,
                    id_usuario = userId,
                    ativo = true,
                    data_entrada = DateTime.UtcNow,
                    iniciou_passeio = false,
                    ultima_atividade = DateTime.UtcNow
                };

                _context.GrupoUsuario.Add(grupoUsuario);


                // -------------------------------------------------
                // MARCA PASSEIO COMO PENDENTE
                // -------------------------------------------------

                var pendente = new PasseioPendente
                {
                    id_usuario = userId,
                    id_passeio = dto.PasseioId,
                    id_grupo = grupo.id_grupo,
                    data_adicao = DateTime.UtcNow
                };

                _context.PasseioPendente.Add(pendente);


                await _context.SaveChangesAsync();


                return Ok(new
                {
                    idGrupo = grupo.id_grupo,
                    nome = grupo.nome,
                    codigoConvite = grupo.codigo_convite,
                    idPasseio = grupo.id_passeio,
                    dataInicio = grupo.data_inicio,
                    status = grupo.status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"Erro ao criar grupo: {ex.Message}");
            }
        }


        // =========================================================
        // VALIDAR CÓDIGO
        // =========================================================

        [HttpGet("validar")]
        public async Task<ActionResult> ValidarGrupo(
            string codigo)
        {
            var grupo = await _context.Grupo
                .FirstOrDefaultAsync(
                    g => g.codigo_convite == codigo);

            if (grupo == null)
            {
                return NotFound(new
                {
                    valido = false,
                    mensagem = "Grupo não encontrado."
                });
            }

            if (grupo.status == "FINALIZADO")
            {
                return BadRequest(new
                {
                    valido = false,
                    mensagem = "Este grupo já foi finalizado."
                });
            }

            return Ok(new
            {
                valido = true,
                idGrupo = grupo.id_grupo,
                nome = grupo.nome,
                idPasseio = grupo.id_passeio,
                dataInicio = grupo.data_inicio
            });
        }


        // =========================================================
        // ENTRAR NO GRUPO
        // =========================================================

        [Authorize]
        [HttpPost("entrar")]
        public async Task<ActionResult> EntrarGrupo(
            [FromBody] EntrarGrupoDto dto)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var grupo = await _context.Grupo
                .FirstOrDefaultAsync(
                    g => g.codigo_convite == dto.CodigoConvite);

            if (grupo == null)
                return NotFound("Grupo não encontrado.");

            if (grupo.status == "FINALIZADO")
                return BadRequest(
                    "Este grupo já foi finalizado.");


            // Verifica se já participa

            var membroExistente =
                await _context.GrupoUsuario
                    .FirstOrDefaultAsync(
                        gu =>
                            gu.id_grupo == grupo.id_grupo &&
                            gu.id_usuario == userId);

            if (membroExistente != null)
            {
                if (!membroExistente.ativo)
                {
                    membroExistente.ativo = true;
                    membroExistente.ultima_atividade =
                        DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    mensagem = "Você já faz parte deste grupo.",
                    idGrupo = grupo.id_grupo
                });
            }


            // Novo integrante

            var novoMembro = new GrupoUsuario
            {
                id_grupo = grupo.id_grupo,
                id_usuario = userId,
                ativo = true,
                data_entrada = DateTime.UtcNow,
                iniciou_passeio = false,
                ultima_atividade = DateTime.UtcNow
            };

            _context.GrupoUsuario.Add(novoMembro);


            // O passeio também passa a aparecer
            // como pendente para esse usuário.

            var pendenteExistente =
                await _context.PasseioPendente
                    .AnyAsync(pp =>
                        pp.id_usuario == userId &&
                        pp.id_passeio == grupo.id_passeio &&
                        pp.id_grupo == grupo.id_grupo);

            if (!pendenteExistente)
            {
                _context.PasseioPendente.Add(
                    new PasseioPendente
                    {
                        id_usuario = userId,
                        id_passeio = grupo.id_passeio,
                        id_grupo = grupo.id_grupo,
                        data_adicao = DateTime.UtcNow
                    });
            }


            await _context.SaveChangesAsync();


            return Ok(new
            {
                mensagem = "Você entrou no grupo.",
                idGrupo = grupo.id_grupo
            });
        }


        // =========================================================
        // DETALHES DO GRUPO
        // =========================================================

        [Authorize]
        [HttpGet("{idGrupo}")]
        public async Task<ActionResult> DetalhesGrupo(
            int idGrupo)
        {
            var grupo = await _context.Grupo
                .Include(g => g.Passeio)
                .Include(g => g.Criador)
                .Include(g => g.Usuarios!)
                    .ThenInclude(gu => gu.Usuario)
                .FirstOrDefaultAsync(
                    g => g.id_grupo == idGrupo);

            if (grupo == null)
                return NotFound("Grupo não encontrado.");


            var resultado = new
            {
                idGrupo = grupo.id_grupo,

                nome = grupo.nome,

                codigoConvite = grupo.codigo_convite,

                status = grupo.status,

                idPasseio = grupo.id_passeio,

                passeio = grupo.Passeio == null
                    ? null
                    : new
                    {
                        id = grupo.Passeio.id_passeio,
                        nome = grupo.Passeio.nome_passeio,
                        descricao = grupo.Passeio.descricao,
                        imagemUrl = grupo.Passeio.img_url
                    },

                dataCriacao = grupo.data_criacao,

                dataInicio = grupo.data_inicio,

                criadorId = grupo.id_criador,

                integrantes = grupo.Usuarios?
                    .Where(u => u.ativo)
                    .Select(u => new
                    {
                        idUsuario = u.id_usuario,

                        nome = u.Usuario != null
                            ? u.Usuario.nome_completo
                            : "Usuário",

                        iniciouPasseio =
                            u.iniciou_passeio,

                        dataInicioPasseio =
                            u.data_inicio_passeio,

                        ultimaAtividade =
                            u.ultima_atividade,

                        online =
                            u.ultima_atividade.HasValue &&
                            u.ultima_atividade.Value
                                >= DateTime.UtcNow.AddMinutes(-2)
                    })
                    .ToList()
            };

            return Ok(resultado);
        }


        // =========================================================
        // INICIAR PASSEIO
        // =========================================================

        [Authorize]
        [HttpPost("{idGrupo}/iniciar")]
        public async Task<ActionResult> IniciarPasseio(
            int idGrupo)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);


            var membro = await _context.GrupoUsuario
                .FirstOrDefaultAsync(
                    gu =>
                        gu.id_grupo == idGrupo &&
                        gu.id_usuario == userId &&
                        gu.ativo);

            if (membro == null)
            {
                return BadRequest(
                    "Você não faz parte deste grupo.");
            }


            var grupo = await _context.Grupo
                .FirstOrDefaultAsync(
                    g => g.id_grupo == idGrupo);

            if (grupo == null)
                return NotFound("Grupo não encontrado.");


            if (grupo.status == "FINALIZADO")
            {
                return BadRequest(
                    "Este passeio já foi finalizado.");
            }


            // Usuário já iniciou

            if (membro.iniciou_passeio)
            {
                return Ok(new
                {
                    mensagem =
                        "Você já iniciou o passeio.",

                    iniciouPasseio = true,

                    dataInicioPasseio =
                        membro.data_inicio_passeio
                });
            }


            // Marca o usuário como iniciado

            membro.iniciou_passeio = true;

            membro.data_inicio_passeio =
                DateTime.UtcNow;

            membro.ultima_atividade =
                DateTime.UtcNow;


            // -------------------------------------------------
            // O PRIMEIRO INTEGRANTE QUE APERTAR INICIAR
            // inicia o grupo.
            // -------------------------------------------------

            if (grupo.status == "CRIADO")
            {
                grupo.status = "EM_ANDAMENTO";

                if (!grupo.data_inicio.HasValue)
                {
                    grupo.data_inicio =
                        DateTime.UtcNow;
                }
            }


            await _context.SaveChangesAsync();


            return Ok(new
            {
                mensagem =
                    "Passeio iniciado para este integrante.",

                iniciouPasseio = true,

                grupoIniciado =
                    grupo.status == "EM_ANDAMENTO",

                status = grupo.status,

                dataInicioPasseio =
                    membro.data_inicio_passeio,

                dataInicioGrupo =
                    grupo.data_inicio
            });
        }
    }
}