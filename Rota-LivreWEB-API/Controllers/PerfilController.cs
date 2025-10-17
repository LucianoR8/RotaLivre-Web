using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Repositories;
using Rota_LivreWEB_API.Models;

namespace Rota_LivreWEB_API.Controllers
{
    public class PerfilController : Controller
    {
        private readonly UsuarioRepository _usuarioRp;

        public PerfilController(UsuarioRepository usuarioRp)
        {
            _usuarioRp = usuarioRp;
        }

        public async Task<ActionResult> Perfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            return View(usuario);
        }

        public async Task<ActionResult> Editar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            var model = new UsuarioEdicaoViewModel
            {
                id_usuario = usuario.id_usuario,
                nome_completo = usuario.nome_completo,
                email = usuario.email,
                data_nasc = usuario.data_nasc
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(UsuarioEdicaoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); 

            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Login");

            var usuario = await _usuarioRp.BuscarPorIdAsync(idUsuario.Value);
            if (usuario == null) return NotFound();

            usuario.nome_completo = model.nome_completo;
            usuario.email = model.email;
            usuario.data_nasc = model.data_nasc;

            await _usuarioRp.AtualizarUsuarioAsync(usuario);

            return RedirectToAction("Perfil");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Deletar()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario != null)
            {
                await _usuarioRp.DeletarUsuarioAsync(idUsuario.Value);
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login", "Login");
        }
    }
}
