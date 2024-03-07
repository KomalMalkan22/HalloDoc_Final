using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckAdminAccess]
    public class MyProfileController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/MyProfile/Index");
        }
    }
}
