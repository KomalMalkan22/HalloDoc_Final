using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckAdminAccess]
    public class ProvidersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Providers/Index");
        }
    }
}
