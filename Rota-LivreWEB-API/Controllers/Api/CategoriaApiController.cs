using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            // Traz apenas as ativas para não quebrar o app
            var categorias = await _context.Categoria.Where(c => c.ativo).ToListAsync();
            return Ok(categorias);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCategoria([FromBody] Categoria categoria)
        {
            categoria.atualizado_em = DateTime.UtcNow;
            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.id_categoria }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCategoria(int id, [FromBody] Categoria categoriaAtualizada)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.tipo_categoria = categoriaAtualizada.tipo_categoria;
            categoria.img = categoriaAtualizada.img;
            categoria.ativo = categoriaAtualizada.ativo;
            categoria.atualizado_por = categoriaAtualizada.atualizado_por;
            categoria.atualizado_em = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            // Em vez de deletar do banco, fazemos um "Soft Delete"
            categoria.ativo = false;
            categoria.atualizado_em = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Categoria desativada com sucesso." });
        }
    }
}