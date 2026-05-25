using Microsoft.AspNetCore.Mvc;

namespace Rota_LivreWEB_API.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}
