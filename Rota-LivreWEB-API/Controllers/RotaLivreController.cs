using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.DbContext;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rota_LivreWEB_API.Controllers
{
    
    public class RotaLivreController : Controller
    {
        private readonly ILogger<RotaLivreController> _logger;

        public RotaLivreController(ILogger<RotaLivreController> logger)
        {
            _logger = logger;
        }

        
        public ViewResult Create()
        {
            var perguntas = UsuarioDbContext.ObterPerguntasDeSeguranca();
            ViewBag.Perguntas = new SelectList(perguntas, "id_pergunta", "pergunta_seg");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (UsuarioDbContext.EmailExiste(usuario.email))
                {
                    ViewBag.MensagemErro = "Este e-mail j� est� cadastrado!";
                    return View(usuario); 
                }

                UsuarioDbContext.Cadastra_Usuario(usuario); 
                return RedirectToAction("CadastroConcluido");
            }

            var perguntas = UsuarioDbContext.ObterPerguntasDeSeguranca();
            ViewBag.Perguntas = new SelectList(perguntas, "id_pergunta", "pergunta_seg");

            return View(usuario);
        }

        [HttpPost("Cadastrar_Usuario")]
        public ActionResult CadastrarUsuario(string Novo_Usuario_Nome, string Novo_Usuario_Nasc, string Novo_Usuario_Email, string Novo_Usuario_Senha, string Novo_Usuario_Resposta_Seg)
        {
            Usuario NovoUsuario = new Usuario(Novo_Usuario_Nome, Convert.ToDateTime(Novo_Usuario_Nasc), Novo_Usuario_Email, Novo_Usuario_Senha, Novo_Usuario_Resposta_Seg);

            if (UsuarioDbContext.Cadastra_Usuario(NovoUsuario) == "Sucesso")
            {
                return Ok("Cadastro concluido parabens");
            }
            else
            {
                return BadRequest("Falha ao concluir o cadastro tente novamente mais tarde");
            }
        }

        public ViewResult CadastroConcluido()
        {
            return View();
        }

        










    }
}