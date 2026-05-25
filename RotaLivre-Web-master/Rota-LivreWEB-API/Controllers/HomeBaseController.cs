using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Rota_LivreWEB_API.Controllers
{
    public class HomeBaseController : Controller
    {
        private readonly UsuarioRepository _usuarioRp;
        private readonly CategoriaRepository _categoriaRp;
        private readonly PasseioRepository _passeioRp;

        public HomeBaseController(UsuarioRepository usuarioRp, CategoriaRepository categoriaRp, PasseioRepository passeioRp)
        {
            _usuarioRp = usuarioRp;
            _categoriaRp = categoriaRp;
            _passeioRp = passeioRp;
        }

        public async Task<ViewResult> Home()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            ViewBag.NomeUsuario = "Visitante";

            if (idUsuario != null)
            {
                var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
                ViewBag.NomeUsuario = usuario?.nome_completo ?? "Usuário";
            }

            var categorias = await _categoriaRp.ObterCategoriasAsync();
            var passeiosEmDestaque = await _passeioRp.BuscarPasseiosMaisCurtidosAsync();

            ViewBag.PasseiosDestaque = passeiosEmDestaque;
            return View(categorias);
        }

        public async Task<ActionResult> PasseiosPorCategoria(int id)
        {
            var passeios = await _passeioRp.BuscarPasseiosPorCategoriaAsync(id);
            ViewBag.CategoriaId = id;
            return View(passeios);
        }
    }
}
