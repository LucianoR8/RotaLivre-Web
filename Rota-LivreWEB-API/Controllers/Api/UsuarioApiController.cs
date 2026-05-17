using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Utilidades.Seguranca;
using Supabase;
using System.Net.Http.Headers;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioApiController : ControllerBase
    {
        private readonly UsuarioRepository _repo;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsuarioApiController(UsuarioRepository repo, AppDbContext context, IConfiguration config)
        {
            _repo = repo;
            _context = context;
            _config = config;
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> Cadastrar([FromBody] UsuarioCadastroDto dto)
        {
            if (_repo.EmailExiste(dto.Email))
            {
                return BadRequest("EMAIL_JA_EXISTE");
            }

            var usuario = new Usuario
            {
                nome_completo = dto.Nome,
                data_nasc = DateOnly.Parse(dto.DataNasc),
                email = dto.Email,
                senha = dto.Senha,
                id_pergunta = dto.IdPergunta,
                resposta_seg = dto.RespostaSeg,
            };

            var sucesso = await _repo.CadastrarAsync(usuario);

            if (sucesso)
                return Ok();

            return BadRequest("ERRO_AO_CADASTRAR");
        }

        [HttpGet("perguntas")]
        public async Task<ActionResult> GetPerguntas()
        {
            var perguntas = await _context.PerguntaSeguranca
                .Select(p => new
                {
                    p.id_pergunta,
                    p.pergunta_seg
                })
                .ToListAsync();

            return Ok(perguntas);
        }

        [HttpGet("perfil/{id}")]
        public async Task<ActionResult> GetPerfil(int id)
        {
            var usuario = await _repo.BuscarPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            var dto = new UsuarioPerfilDto
            {
                Id = usuario.id_usuario,
                Nome = usuario.nome_completo,
                Email = usuario.email,
                DataNasc = usuario.data_nasc.ToString("yyyy-MM-dd"),
                FotoPerfilUrl = usuario.FotoPerfilUrl
            };

            return Ok(dto);
        }

        [HttpPut("editar")]
        public async Task<ActionResult> Editar([FromBody] UsuarioPerfilDto dto)
        {
            var usuario = await _repo.BuscarPorIdAsync(dto.Id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            var emailJaExiste = _context.Usuario
                .Any(u => u.email == dto.Email && u.id_usuario != dto.Id);

            if (emailJaExiste)
                return BadRequest("EMAIL_JA_EXISTE");

            usuario.nome_completo = dto.Nome;
            usuario.email = dto.Email;
            usuario.data_nasc = DateOnly.Parse(dto.DataNasc);

            await _repo.AtualizarUsuarioAsync(usuario);

            return Ok();
        }

        [HttpDelete("deletar/{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            await _repo.DeletarUsuarioAsync(id);
            return Ok();
        }



        [HttpGet("pergunta")]
        public ActionResult BuscarPergunta([FromQuery] string email)
        {
            var usuario = _repo.BuscarUsuarioPorEmail(email);

            if (usuario == null)
                return NotFound(new
                {
                    mensagem = "Usuário não encontrado"
                });

            var pergunta = _repo.BuscarPerguntaDoUsuario(email);

            return Ok(new
            {
                pergunta
            });
        }

        [HttpPost("verificar-resposta")]
        public ActionResult VerificarResposta([FromBody] VerificarRespostaDto dto)
        {
            string respostaHash =
                HashHelper.GerarHash(dto.Resposta);

            string respostaBanco =
                _repo.ObterRespostaDoBanco(dto.Email);

            if (respostaHash != respostaBanco)
            {
                return BadRequest(new
                {
                    mensagem = "Resposta incorreta"
                });
            }

            return Ok(new
            {
                sucesso = true
            });
        }

        [HttpPost("redefinir")]
        public ActionResult RedefinirSenha([FromBody] RedefinirSenhaDto dto)
        {
            var usuario =
                _repo.BuscarUsuarioPorEmail(dto.Email);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensagem = "Usuário não encontrado"
                });
            }

            bool sucesso =
                _repo.AlterarSenha(
                    usuario.id_usuario,
                    dto.NovaSenha);

            if (!sucesso)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao alterar senha"
                });
            }

            return Ok(new
            {
                mensagem = "Senha alterada com sucesso"
            });
        }

        [HttpPost("upload-foto/{id}")]
        public async Task<ActionResult> UploadFoto(int id, IFormFile foto)
        {
            var usuario = await _repo.BuscarPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            if (foto == null || foto.Length == 0)
                return BadRequest("Imagem inválida");

            var supabaseUrl = _config["Supabase:Url"];
            var supabaseKey = _config["Supabase:Key"];
            var bucket = _config["Supabase:Bucket"];

            var fileName = $"usuario_{id}_{Guid.NewGuid()}.jpg";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", supabaseKey);

            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

            using var stream = foto.OpenReadStream();

            var content = new StreamContent(stream);

            content.Headers.ContentType =
                new MediaTypeHeaderValue(foto.ContentType);

            var response = await httpClient.PostAsync(
                $"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                return BadRequest(erro);
            }

            var urlPublica =
                $"{supabaseUrl}/storage/v1/object/public/{bucket}/{fileName}";

            usuario.FotoPerfilUrl = urlPublica;

            await _repo.AtualizarUsuarioAsync(usuario);

            return Ok(new
            {
                fotoUrl = urlPublica
            });
        }

        [HttpDelete("remover-foto/{id}")]
        public async Task<IActionResult> RemoverFoto(int id)
        {
            var usuario = await _repo.BuscarPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            if (string.IsNullOrEmpty(usuario.FotoPerfilUrl))
                return BadRequest("Usuário não possui foto.");

            var fileName = usuario.FotoPerfilUrl
                .Split('/')
                .Last();

            var supabaseUrl = _config["Supabase:Url"];
            var supabaseKey = _config["Supabase:Key"];
            var bucket = _config["Supabase:Bucket"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", supabaseKey);

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{supabaseUrl}/storage/v1/object/{bucket}/{fileName}");

            var response = await client.SendAsync(request);

            usuario.FotoPerfilUrl = null;

            await _repo.AtualizarUsuarioAsync(usuario);

            return Ok();
        }

    }
}
