using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;

namespace Rota_LivreWEB_API.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string senha)
        {
            Console.WriteLine($"Tentando login: {email} - {senha}"); // para teste no terminal

            if (UsuarioDbContext.VerificarLogin(email, senha))
            {
                // 🧠 Buscar ID do usuário logado (ou nome, se preferir)
                int idUsuario = UsuarioDbContext.BuscarIdPorEmail(email);

                // 🧠 Salvar o ID na sessão
                HttpContext.Session.SetInt32("IdUsuario", idUsuario);

                // ✅ Redirecionamento CORRETO
                return RedirectToAction("Home", "HomeBase");
            }
            else
            {
                ViewBag.MensagemErro = "Email ou senha inválidos!";
                return View();
            }
        }
    }
}
