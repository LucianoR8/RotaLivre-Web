using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;

namespace Rota_LivreWEB_API.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly CategoriaRepository _categoriaRp;

        public CategoriaController(CategoriaRepository categoriaRp)
        {
            _categoriaRp = categoriaRp;
        }

        public async Task<ActionResult> Index()
        {
            var categorias = await _categoriaRp.ObterCategoriasAsync();
            return View(categorias);
        }
    }
}
