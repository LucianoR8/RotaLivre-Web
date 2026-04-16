using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DTOs;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizacaoController : ControllerBase
    {
        private readonly LocalizacaoRepository _repository;

        public LocalizacaoController(LocalizacaoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> ReceberLocalizacao([FromBody] LocalizacaoDto dto)
        {
            if (dto == null) return BadRequest("Dados inválidos");

            // Transformamos o DTO (pacote) no Model (entidade do banco)
            var novaLocalizacao = new Localizacao
            {
                latitude = dto.Latitude,
                longitude = dto.Longitude,
                data_registro = dto.DataAtualizacao == default ? DateTime.Now : dto.DataAtualizacao
            };

            // Chamamos o repositório para salvar nas duas tabelas
            var sucesso = await _repository.SalvarHistoricoAsync(novaLocalizacao, dto.UsuarioId);

            if (sucesso)
            {
                return Ok(new { mensagem = "Localização salva com sucesso!" });
            }

            return StatusCode(500, "Erro ao salvar localização no servidor.");
        }
        // pra testar se o histórico tá funcionando, depois a gente pode retirar esse endpoint
        [HttpGet("historico/{usuarioId}")]
        public async Task<IActionResult> ObterHistorico(int usuarioId)
        {
            // Chama o repositório para buscar a lista no banco
            var historico = await _repository.ObterHistoricoPorUsuarioAsync(usuarioId);

            if (historico == null || !historico.Any())
            {
                return NotFound("Nenhum histórico de localização encontrado para este usuário.");
            }

            // Retorna a lista de localizações em formato JSON
            return Ok(historico);
        }
    }
}
