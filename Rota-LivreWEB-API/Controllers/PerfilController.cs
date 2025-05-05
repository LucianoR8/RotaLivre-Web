using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class PerfilController : Controller
    {
        public IActionResult Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Login"); // se não estiver logado
            }

            Usuario usuario = UsuarioDbContext.BuscarUsuarioPorId(idUsuario.Value);

            return View(usuario); // envia os dados pra view
        }

        public IActionResult Editar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = UsuarioDbContext.BuscarUsuarioPorId(idUsuario.Value);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Usuario usuario)
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
        public IActionResult Deletar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario != null)
            {
                UsuarioDbContext.DeletarUsuario(idUsuario.Value);
                HttpContext.Session.Clear(); 
            }

            return RedirectToAction("Login", "Login");
        }

        public IActionResult DetalhesPerfil()
        {
            int trofeus = 0;
            Console.WriteLine("Valor dos troféus: " + trofeus); // <-- Aqui imprime no terminal
            return View();
        }



    }



}
