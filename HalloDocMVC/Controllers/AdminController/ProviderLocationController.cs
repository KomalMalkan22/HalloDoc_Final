using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class ProviderLocationController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/ProviderLocation/Index");
        }
    }
}
