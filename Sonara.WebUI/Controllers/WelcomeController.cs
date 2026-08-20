using Microsoft.AspNetCore.Mvc;

namespace Sonara.WebUI.Controllers
{
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}