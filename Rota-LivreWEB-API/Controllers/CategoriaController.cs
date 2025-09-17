using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;

namespace Rota_LivreWEB_API.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly PasseioDb _passeioDb;
        public CategoriaController(PasseioDb passeioDb)
        {
            _passeioDb = passeioDb;
        }
        public ActionResult Index()
        {
            var categorias = _passeioDb.BuscarCategorias();
            return View(categorias);
        }
    }

}
