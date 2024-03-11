using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class ProvidersController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Providers/Index");
        }
    }
}
