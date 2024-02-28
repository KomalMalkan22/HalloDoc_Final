using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ActionsController : Controller
    {
        private readonly IActions _IActions;
        public ActionsController(IActions iActions)
        {
            _IActions = iActions;
        }
        public async Task<IActionResult> ViewCase(int Id)
        {
            ViewCaseModel vcm = _IActions.GetRequestForViewCase(Id);
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
        }
        public IActionResult EditCase(ViewCaseModel vcm)
        {
            bool result = _IActions.EditCase(vcm);
            if (result)
            {
                return RedirectToAction("ViewCase", "Actions", new { id = vcm.RequestId });
            }
            else
            {
                return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
            }
        }
        public async Task<IActionResult> ViewNotes()
        {
            return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml");
        }
    }
}
