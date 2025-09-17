using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Data;

namespace Rota_LivreWEB_API.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioDbContext _usuarioDb;

        public LoginController(UsuarioDbContext usuarioDb)
        {
            _usuarioDb = usuarioDb;
        }
        public ViewResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string senha)
        {
            Console.WriteLine($"Tentando login: {email} - {senha}"); 

            if (_usuarioDb.VerificarLogin(email, senha))
            {
                
                int idUsuario = _usuarioDb.BuscarIdPorEmail(email);
                string nome_completo = _usuarioDb.BuscarNomePorEmail(email);


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

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }


        public ViewResult SolicitarRedefinicao()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SolicitarRedefinicaoSenha(string email)
        {
            var usuario = _usuarioDb.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                ViewBag.Erro = "E-mail não encontrado!";
                return View("SolicitarRedefinicao");
            }


            return RedirectToAction("ConfirmarResposta", new { email = email });
        }

        public ViewResult DefinirNovaSenha(int id)
        {
            ViewBag.IdUsuario = id;
            return View();
        }

        [HttpPost]
        public ActionResult DefinirNovaSenha(int id_usuario, string novaSenha, string confirmarSenha)
        {
            if (novaSenha != confirmarSenha)
            {
                ViewBag.Erro = "As senhas não coincidem!";
                ViewBag.IdUsuario = id_usuario;
                return View();
            }

            bool sucesso = _usuarioDb.AlterarSenha(id_usuario, novaSenha);

            if (sucesso)
                return RedirectToAction("SenhaAlteradaSucesso");
            else
                return StatusCode(500, "Erro ao redefinir senha.");
        }

        public ActionResult ConfirmarResposta(string email)
        {
            var pergunta = _usuarioDb.BuscarPerguntaDoUsuario(email);
            ViewBag.Email = email;
            ViewBag.Pergunta = pergunta;
            return View();
        }

        [HttpPost]
        public ActionResult VerificarResposta(string email, string RespostaInformada)
        {
            string respostaHash = UsuarioDbContext.GerarHash(RespostaInformada);
            string respostaBanco = _usuarioDb.ObterRespostaDoBanco(email);

            if (respostaHash == respostaBanco)
            {
                var usuario = _usuarioDb.BuscarUsuarioPorEmail(email);
                return RedirectToAction("DefinirNovaSenha", new { id = usuario.id_usuario });
            }
            else
            {
                ViewBag.Erro = "Resposta incorreta.";
                var pergunta = _usuarioDb.BuscarPerguntaDoUsuario(email);
                ViewBag.Email = email;
                ViewBag.Pergunta = pergunta;
                return View("ConfirmarResposta");
            }
        }




        public ViewResult SenhaAlteradaSucesso()
        {
            return View(); 
        }
    }
}
