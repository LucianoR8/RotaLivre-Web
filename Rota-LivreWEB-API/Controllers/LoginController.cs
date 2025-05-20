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
                string nome_completo = UsuarioDbContext.BuscarNomePorEmail(email);


                HttpContext.Session.SetInt32("IdUsuario", idUsuario);
                HttpContext.Session.SetString("NomeUsuario", nome_completo);


                return RedirectToAction("Home", "HomeBase");
            }
            else
            {
                ViewBag.MensagemErro = "Email ou senha inválidos!";
                return View();
            }
        }


         public ViewResult SolicitarRedefinicao()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SolicitarRedefinicaoSenha(string email)
        {
            var usuario = UsuarioDbContext.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                ViewBag.Erro = "E-mail não encontrado!";
                return View("SolicitarRedefinicao");
            }

            
            return RedirectToAction("DefinirNovaSenha", new { id = usuario.id_usuario });
        }

        public ViewResult DefinirNovaSenha(int id)
        {
            ViewBag.IdUsuario = id;
            return View();
        }

        [HttpPost]
        public IActionResult DefinirNovaSenha(int id_usuario, string novaSenha, string confirmarSenha)
        {
            if (novaSenha != confirmarSenha)
            {
                ViewBag.Erro = "As senhas não coincidem!";
                ViewBag.IdUsuario = id_usuario;
                return View();
            }

            bool sucesso = UsuarioDbContext.AlterarSenha(id_usuario, novaSenha);

            if (sucesso)
                return RedirectToAction("SenhaAlteradaSucesso");
            else
                return StatusCode(500, "Erro ao redefinir senha.");
        }

        public ViewResult SenhaAlteradaSucesso()
        {
            return View(); 
        }
    }
}
