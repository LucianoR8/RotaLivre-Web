using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly PasseioRepository _passeioRp;

        public AvaliacaoController(PasseioRepository passeioRp)
        {
            _passeioRp = passeioRp;
        }

        public async Task<ActionResult> Avaliacoes(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? nomeUsuario = HttpContext.Session.GetString("NomeUsuario");

            if (idUsuario == null || string.IsNullOrEmpty(nomeUsuario))
                return RedirectToAction("Login", "Login");

            ViewBag.IdUsuario = idUsuario;
            ViewBag.NomeUsuario = nomeUsuario;
            ViewBag.IdPasseio = idPasseio;

            var avaliacoes = await _passeioRp.ListarAvaliacoesPorPasseioAsync(idPasseio);
            return View(avaliacoes);
        }

        [HttpPost]
        public async Task<ActionResult> Comentar(int id_passeio, int nota, string feedback)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            await _passeioRp.InserirAvaliacaoAsync(id_passeio, idUsuario.Value, nota, feedback);
            return RedirectToAction("Index", new { id = id_passeio });
        }

        public async Task<ViewResult> Index(int id)
        {
            ViewBag.IdPasseio = id;
            var lista = await _passeioRp.ListarAvaliacoesPorPasseioAsync(id);
            return View(lista);
        }
    }
}
