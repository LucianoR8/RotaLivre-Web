using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioApiController : ControllerBase
    {
        private readonly UsuarioRepository _repo;
        private readonly AppDbContext _context;

        public UsuarioApiController(UsuarioRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
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
                DataNasc = usuario.data_nasc.ToString("yyyy-MM-dd")
            };

            return Ok(dto);
        }

        [HttpPut("editar")]
        public async Task<ActionResult> Editar([FromBody] UsuarioPerfilDto dto)
        {
            var usuario = await _repo.BuscarPorIdAsync(dto.Id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            usuario.nome_completo = dto.Nome;
            usuario.email = dto.Email;
            usuario.data_nasc = DateOnly.Parse(dto.DataNasc);

            if (_repo.EmailExiste(dto.Email) && usuario.email != dto.Email)
            {
                return BadRequest("EMAIL_JA_EXISTE");
            }

            await _repo.AtualizarUsuarioAsync(usuario);

            return Ok();
        }

        [HttpDelete("deletar/{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            await _repo.DeletarUsuarioAsync(id);
            return Ok();
        }


    }
}
