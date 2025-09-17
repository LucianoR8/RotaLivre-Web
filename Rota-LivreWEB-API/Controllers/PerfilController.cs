using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class PerfilController : Controller
    {
        private readonly UsuarioDbContext _usuarioDb;
        public PerfilController(UsuarioDbContext usuarioDb)
        {
            _usuarioDb = usuarioDb;
        }
        public ActionResult Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Login"); 
            }

            Usuario usuario = _usuarioDb.BuscarUsuarioPorId(idUsuario.Value);

            return View(usuario); 
        }

        public ActionResult Editar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = _usuarioDb.BuscarUsuarioPorId(idUsuario.Value);
            var vm = new UsuarioEdicaoViewModel
            {
                id_usuario = usuario.id_usuario,
                nome_completo = usuario.nome_completo,
                email = usuario.email,
                data_nasc = usuario.data_nasc
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UsuarioEdicaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                if (idUsuario == null) return RedirectToAction("Login", "Login");

                var usuarioExistente = _usuarioDb.BuscarUsuarioPorId(idUsuario.Value);

                if (usuarioExistente != null)
                {
                    
                    usuarioExistente.nome_completo = model.nome_completo;
                    usuarioExistente.email = model.email;
                    usuarioExistente.data_nasc = model.data_nasc;

                    _usuarioDb.AtualizarUsuario(usuarioExistente);
                }

                return RedirectToAction("Perfil");
            }

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario != null)
            {
                _usuarioDb.DeletarUsuario(idUsuario.Value);
                HttpContext.Session.Clear(); 
            }

            return RedirectToAction("Login", "Login");
        }

       



    }



}
