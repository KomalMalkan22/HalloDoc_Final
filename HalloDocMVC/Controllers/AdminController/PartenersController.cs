using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckAdminAccess]
    public class PartenersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Parteners/Index");
        }
    }
}
