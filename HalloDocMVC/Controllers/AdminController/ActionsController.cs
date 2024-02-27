using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ActionsController : Controller
    {
        public async Task<IActionResult> ViewCase()
        {
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml");
        }
        public async Task<IActionResult> ViewNotes()
        {
            return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml");
        }
    }
}
