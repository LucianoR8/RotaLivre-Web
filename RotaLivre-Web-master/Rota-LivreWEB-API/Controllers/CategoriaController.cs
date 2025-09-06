using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;

namespace Rota_LivreWEB_API.Controllers
{
    public class CategoriaController : Controller
    {
        public ActionResult Index()
        {
            var categorias = PasseioDb.BuscarCategorias();
            return View(categorias);
        }
    }

}
