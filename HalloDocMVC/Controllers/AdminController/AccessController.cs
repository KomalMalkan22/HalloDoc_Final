using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Access/Index");
        }
    }
}
