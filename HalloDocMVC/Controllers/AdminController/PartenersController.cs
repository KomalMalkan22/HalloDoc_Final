using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class PartenersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Parteners/Index");
        }
    }
}
