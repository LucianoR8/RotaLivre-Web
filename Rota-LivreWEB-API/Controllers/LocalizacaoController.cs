using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;
using System.Security.Claims;

namespace Rota_LivreWEB_API.Controllers
{
    [ApiController]
    [Route("api/localizacao")]
    [Authorize]
    public class LocalizacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocalizacaoController(AppDbContext context)
        {
            _context = context;
        }

        public class SyncLocalizacaoDto
        {
            public int IdGrupo { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        [HttpPost("sync")]
        public async Task<ActionResult> SyncLocalizacao([FromBody] SyncLocalizacaoDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            // Verifica se o usuário realmente está ativo no grupo e já iniciou
            var membro = await _context.GrupoUsuario
                .FirstOrDefaultAsync(gu => gu.id_grupo == dto.IdGrupo && gu.id_usuario == userId && gu.ativo && gu.iniciou_passeio);

            if (membro == null) return Forbid("Usuário não ativo neste grupo.");

            // Atualiza a "ultima_atividade" para ele aparecer como 'Online' nos detalhes do grupo
            membro.ultima_atividade = DateTime.UtcNow;

            // Busca se já existe um registro de localização (Lógica do UPSERT)
            var localizacao = await _context.GrupoLocalizacao // Adapte para o nome exato do seu DbSet
                .FirstOrDefaultAsync(gl => gl.id_grupo == dto.IdGrupo && gl.id_usuario == userId);

            if (localizacao != null)
            {
                // UPDATE
                localizacao.latitude = dto.Latitude;
                localizacao.longitude = dto.Longitude;
            }
            else
            {
                // INSERT
                localizacao = new GrupoLocalizacao // Adapte para sua Model
                {
                    id_grupo = dto.IdGrupo,
                    id_usuario = userId,
                    latitude = dto.Latitude,
                    longitude = dto.Longitude
                };
                _context.GrupoLocalizacao.Add(localizacao);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}