using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly PasseioDb _passeioDb;
        public AvaliacaoController(PasseioDb passeioDb)
        {
            _passeioDb = passeioDb;
        }

        public ActionResult Avaliacoes(int idPasseio)
        { 

            

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? nomeUsuario = HttpContext.Session.GetString("NomeUsuario");

            if (idUsuario == null || string.IsNullOrEmpty(nomeUsuario))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.IdUsuario = idUsuario;
            ViewBag.NomeUsuario = nomeUsuario;
            ViewBag.IdPasseio = idPasseio;

            var avaliacoes = _passeioDb.ListarAvaliacoesPorPasseio(idPasseio);

            return View(avaliacoes);
        }

        [HttpPost]
        public ActionResult Comentar(int id_passeio, int nota, string feedback)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Login");
            }

            Console.WriteLine($"ID_PASSEIO recebido: {id_passeio}");

            _passeioDb.InserirAvaliacao(id_passeio, idUsuario.Value, nota, feedback);

            return RedirectToAction("Index", new { id = id_passeio });
        }

        public ViewResult Index(int id)
        {
            ViewBag.IdPasseio = id;
            var lista = _passeioDb.ListarAvaliacoesPorPasseio(id);
            return View(lista);
        }



    }
}
