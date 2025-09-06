using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.DbContext;

namespace Rota_LivreWEB_API.Controllers
{
    public class MeusPasseiosController : Controller
    {
        public ActionResult MeusPasseios()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var model = new MeusPasseiosViewModel
            {
                Curtidos = PasseioDb.BuscarPasseiosCurtidosPorUsuario(idUsuario.Value),
                Pendentes = PasseioDb.BuscarPasseiosPendentesPorUsuario(idUsuario.Value)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AdicionarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            PasseioDb.AdicionarPasseioPendente(idUsuario.Value, idPasseio);
            return RedirectToAction("MeusPasseios");
        }


        [HttpPost]
        public JsonResult AlternarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado" });

            bool jaPendente = PasseioDb.VerificarPasseioPendente(idUsuario.Value, idPasseio);

            if (jaPendente)
            {
                PasseioDb.RemoverPasseioPendente(idUsuario.Value, idPasseio);
            }
            else
            {
                PasseioDb.AdicionarPasseioPendente(idUsuario.Value, idPasseio);
            }

            return Json(new
            {
                sucesso = true,
                pendente = !jaPendente
            });
        }


    }
}
