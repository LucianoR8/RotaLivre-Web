using Microsoft.AspNetCore.Mvc;
using Rota_LivreWEB_API.DbContext;


namespace Rota_LivreWEB_API.Controllers
{
    public class HomeBaseController : Controller
    {
        public IActionResult Home()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario != null)
            {
                var usuario = UsuarioDbContext.BuscarUsuarioPorId(idUsuario.Value);
                ViewBag.NomeUsuario = usuario?.nome_completo ?? "Usuário";
            }
            else
            {
                ViewBag.NomeUsuario = "Visitante";
            }

            return View();
        }
    }
 }

