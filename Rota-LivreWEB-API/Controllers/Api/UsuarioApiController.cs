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

            return BadRequest();
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
    }
}
