using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckAdminAccess]
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Access/Index");
        }
    }
}
