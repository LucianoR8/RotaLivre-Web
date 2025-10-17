using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class MeusPasseiosController : Controller
    {
        private readonly PasseioRepository _passeioRp;

        public MeusPasseiosController(PasseioRepository passeioRp)
        {
            _passeioRp = passeioRp;
        }

        public async Task<ActionResult> MeusPasseios()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var model = new MeusPasseiosViewModel
            {
                Curtidos = await _passeioRp.BuscarPasseiosCurtidosPorUsuarioAsync(idUsuario.Value),
                Pendentes = await _passeioRp.BuscarPasseiosPendentesPorUsuarioAsync(idUsuario.Value)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AdicionarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            await _passeioRp.AdicionarPasseioPendenteAsync(idUsuario.Value, idPasseio);
            return RedirectToAction("MeusPasseios");
        }

        [HttpPost]
        public async Task<JsonResult> AlternarPendente(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado" });

            bool jaPendente = await _passeioRp.VerificarPasseioPendenteAsync(idUsuario.Value, idPasseio);

            if (jaPendente)
                await _passeioRp.RemoverPasseioPendenteAsync(idUsuario.Value, idPasseio);
            else
                await _passeioRp.AdicionarPasseioPendenteAsync(idUsuario.Value, idPasseio);

            return Json(new { sucesso = true, pendente = !jaPendente });
        }
    }
}
