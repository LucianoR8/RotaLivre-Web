using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.DbContext;
using System.Linq;
using Rota_LivreWEB_API.Repositories;
using MySql.Data.MySqlClient;





namespace Rota_LivreWEB_API.Controllers
{
    public class PasseiosController : Controller
    {
       


        public ActionResult Categoria(int id)
        {
            var passeios = PasseioDb.BuscarPasseiosPorCategoria(id);

            return View(passeios);
        }

        public ActionResult Detalhes(int id)
        {
            var repo = new PasseioRepository();
            var passeio = repo.ObterPasseioPorId(id);
            if (passeio == null)
            {
                return NotFound();
            }

            passeio.QuantidadeCurtidas = repo.ObterTotalCurtidas(id);

            
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            ViewBag.TesteIdUsuario = idUsuario;
            if (idUsuario != null)
            {
                passeio.UsuarioJaCurtiu = repo.UsuarioJaCurtiu(idUsuario.Value, id);
            }




            return View(passeio);
        }





        [HttpPost("/Passeios/Curtir/{idPasseio}")]
        public ActionResult Curtir(int idPasseio)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return Json(new { sucesso = false, mensagem = "Usuário não autenticado." });
            }

           

            var repo = new PasseioRepository();

            bool curtiuAgora = repo.AlternarCurtida(idUsuario.Value, idPasseio);

            return Json(new { sucesso = true, curtiu = curtiuAgora });
        }

        public ActionResult Buscar(string termo)
        {
            var repo = new PasseioRepository();

            var passeios = string.IsNullOrWhiteSpace(termo)
                ? new List<Passeio>()
                : repo.BuscarPasseioPorNome(termo);

            return PartialView("_CardsPasseios", passeios);
        }





    }

}
