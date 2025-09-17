using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Data;

namespace Rota_LivreWEB_API.Controllers
{
    public class MeusPasseiosController : Controller
    {
        private readonly PasseioDb _passeioDb;
        public MeusPasseiosController(PasseioDb passeioDb)
        {
            _passeioDb = passeioDb;
        }
        public ActionResult MeusPasseios()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var model = new MeusPasseiosViewModel
            {
                Curtidos = _passeioDb.BuscarPasseiosCurtidosPorUsuario(idUsuario.Value),
                Pendentes = _passeioDb.BuscarPasseiosPendentesPorUsuario(idUsuario.Value)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AdicionarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            _passeioDb.AdicionarPasseioPendente(idUsuario.Value, idPasseio);
            return RedirectToAction("MeusPasseios");
        }


        [HttpPost]
        public JsonResult AlternarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado" });

            bool jaPendente = _passeioDb.VerificarPasseioPendente(idUsuario.Value, idPasseio);

            if (jaPendente)
            {
                _passeioDb.RemoverPasseioPendente(idUsuario.Value, idPasseio);
            }
            else
            {
                _passeioDb.AdicionarPasseioPendente(idUsuario.Value, idPasseio);
            }

            return Json(new
            {
                sucesso = true,
                pendente = !jaPendente
            });
        }


    }
}
