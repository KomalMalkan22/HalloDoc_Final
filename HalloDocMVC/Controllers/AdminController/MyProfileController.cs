using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class MyProfileController : Controller
    {
        public IActionResult Index()
        {
            return View("../AdminPanel/MyProfile/Index");
        }
    }
}
