using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class PartenersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Parteners/Index");
        }
    }
}
