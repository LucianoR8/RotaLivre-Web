using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;

namespace Rota_LivreWEB_API.Controllers
{
    public class LoginController : Controller
    {
        
        public ViewResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string senha)
        {
            Console.WriteLine($"Tentando login: {email} - {senha}"); 

            if (UsuarioDbContext.VerificarLogin(email, senha))
            {
                
                int idUsuario = UsuarioDbContext.BuscarIdPorEmail(email);

                
                HttpContext.Session.SetInt32("IdUsuario", idUsuario);

               
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
