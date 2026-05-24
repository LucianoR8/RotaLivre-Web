using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Rota_LivreWEB_API.Controllers
{
    
    public class PasseiosController : Controller
    {
        private readonly PasseioRepository _passeioRp;

        public PasseiosController(PasseioRepository passeioRp)
        {
            _passeioRp = passeioRp;
        }

        public async Task<IActionResult> Index()
        {
            var passeios = await _passeioRp.ObterTodosAsync();
            return View(passeios);
        }

        public async Task<ActionResult> Categoria(int id)
        {
            var passeios = await _passeioRp.BuscarPasseiosPorCategoriaAsync(id);
            return View(passeios);
        }

        public async Task<ActionResult> Detalhes(int id)
        {
            var passeio = await _passeioRp.BuscarPasseioPorIdAsync(id);
            if (passeio == null)
                return NotFound();

            passeio.QuantidadeCurtidas = await _passeioRp.ObterTotalCurtidasAsync(id);

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            ViewBag.TesteIdUsuario = idUsuario;

            if (idUsuario != null)
            {
                passeio.UsuarioJaCurtiu = await _passeioRp.UsuarioJaCurtiuAsync(idUsuario.Value, id);
                passeio.UsuarioJaPendente = await _passeioRp.VerificarPasseioPendenteAsync(idUsuario.Value, id);
            }

            return View(passeio);
        }

        [HttpPost("/Passeios/Curtir/{idPasseio}")]
        public async Task<ActionResult> Curtir(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado." });

            bool curtiuAgora = await _passeioRp.AlternarCurtidaAsync(idUsuario.Value, idPasseio);

            return Json(new { sucesso = true, curtiu = curtiuAgora });
        }

        public async Task<ActionResult> Buscar(string termo)
        {
            var passeios = string.IsNullOrWhiteSpace(termo)
                ? new List<Passeio>()
                : await _passeioRp.BuscarPasseioPorNomeAsync(termo);

            return PartialView("_CardsPasseios", passeios);
        }
    }
}
