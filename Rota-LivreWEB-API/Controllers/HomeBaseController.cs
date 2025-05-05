using Microsoft.AspNetCore.Mvc;

namespace Rota_LivreWEB_API.Controllers
{
    public class HomeBaseController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }
    }
}
