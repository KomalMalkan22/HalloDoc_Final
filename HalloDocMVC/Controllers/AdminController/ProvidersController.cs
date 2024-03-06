using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ProvidersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Providers/Index");
        }
    }
}
