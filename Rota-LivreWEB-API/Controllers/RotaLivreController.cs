using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.Models;
using Rota_LivreWEB_API.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rota_LivreWEB_API.Controllers
{
    
    public class RotaLivreController : Controller
    {

        private readonly UsuarioRepository _usuarioRp;
        private readonly ILogger<RotaLivreController> _logger;

        public RotaLivreController(ILogger<RotaLivreController> logger, UsuarioRepository usuarioRp)
        {
            _logger = logger;
            _usuarioRp = usuarioRp;
        }



        public ViewResult Create()
        {
            var perguntas = _usuarioRp.ObterPerguntasDeSeguranca();
            ViewBag.Perguntas = new SelectList(perguntas, "id_pergunta", "pergunta_seg");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UsuarioCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var perguntasInvalidas = _usuarioRp.ObterPerguntasDeSeguranca();
                ViewBag.Perguntas = new SelectList(perguntasInvalidas, "id_pergunta", "pergunta_seg");
                foreach (var kv in ModelState)
                {
                    if (kv.Value.Errors.Any())
                    {
                        _logger.LogWarning("ModelState erro: {Key} => {Errors}", kv.Key, string.Join("; ", kv.Value.Errors.Select(e => e.ErrorMessage)));
                    }
                }
                return View(model);
            }

            if (_usuarioRp.EmailExiste(model.email))
            {
                ViewBag.MensagemErro = "Este e-mail já está cadastrado!";
                var perguntas = _usuarioRp.ObterPerguntasDeSeguranca();
                ViewBag.Perguntas = new SelectList(perguntas, "id_pergunta", "pergunta_seg");
                return View(model);
            }

            var usuario = new Usuario
            {
                nome_completo = model.nome_completo,
                data_nasc = model.data_nasc,
                email = model.email,
                senha = model.senha,
                resposta_seg = model.resposta_seg,
                id_pergunta = model.id_pergunta
            };

            var sucesso = await _usuarioRp.CadastrarAsync(usuario);
            if (sucesso)
            {
                return RedirectToAction("CadastroConcluido");
            }
            else 
            { 
                ViewBag.MensagemErro = "Ocorreu um erro ao cadastrar. Verifique os dados e tente novamente.";
                var perguntas = _usuarioRp.ObterPerguntasDeSeguranca();
                ViewBag.Perguntas = new SelectList(perguntas, "id_pergunta", "pergunta_seg");
                return View(model);
            }
        }


        [HttpPost("Cadastrar_Usuario")]
        public async Task<ActionResult> CadastrarUsuario(string Novo_Usuario_Nome, string Novo_Usuario_Nasc, string Novo_Usuario_Email, string Novo_Usuario_Senha, string Novo_Usuario_Resposta_Seg)
        {
            Usuario NovoUsuario = new Usuario(
                Novo_Usuario_Nome,
                DateOnly.Parse(Novo_Usuario_Nasc),
                Novo_Usuario_Email,
                Novo_Usuario_Senha,
                Novo_Usuario_Resposta_Seg
            );

            bool sucesso = await _usuarioRp.CadastrarAsync(NovoUsuario);

            if (sucesso)
            {
                return Ok("Cadastro concluído com sucesso!");
            }
            else
            {
                return BadRequest("Falha ao concluir o cadastro. Tente novamente mais tarde.");
            }
        }
        public ViewResult CadastroConcluido()
        {
            return View();
        }


    }
}