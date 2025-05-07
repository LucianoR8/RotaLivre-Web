using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class PerfilController : Controller
    {
        public ActionResult Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Login"); 
            }

            Usuario usuario = UsuarioDbContext.BuscarUsuarioPorId(idUsuario.Value);

            return View(usuario); 
        }

        public ActionResult Editar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = UsuarioDbContext.BuscarUsuarioPorId(idUsuario.Value);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                UsuarioDbContext.AtualizarUsuario(usuario);
                return RedirectToAction("Perfil");
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario != null)
            {
                UsuarioDbContext.DeletarUsuario(idUsuario.Value);
                HttpContext.Session.Clear(); 
            }

            return RedirectToAction("Login", "Login");
        }

       



    }



}
