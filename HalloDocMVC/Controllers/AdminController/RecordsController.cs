using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class RecordsController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/Records/Index");
        }
    }
}
