using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ActionsController : Controller
    {
        #region Configuration
        private readonly IActions _IActions;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;
        public ActionsController(IActions iActions, IComboBox iComboBox, INotyfService iNotyfService, ILogger<ActionsController> logger)
        {
            _IActions = iActions;
            _IComboBox = iComboBox;
            _INotyfService = iNotyfService;
            _logger = logger;
        }
        #endregion Configuration

        #region ViewCase
        public async Task<IActionResult> ViewCase(int Id)
        {
            ViewBag.ComboBoxRegion = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBox.ComboBoxCaseReasons();
            ViewCaseModel vcm = _IActions.GetRequestForViewCase(Id);
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
        }
        #endregion ViewCase

        #region EditCase
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
        #endregion EditCase

        #region AssignProvider
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _IActions.AssignProvider(requestid, ProviderId, Notes))
            {
                _INotyfService.Success("Physician Assigned successfully...");
            }
            else
            {
                _INotyfService.Error("Physician Not Assigned...");
            }
            return RedirectToAction("Index", "DashBoard");
        }
        #endregion

        #region ProviderbyRegion
        public IActionResult ProviderbyRegion(int? Regionid)
        {
            var data = _IComboBox.ProviderByRegion(Regionid);
            return Json(data);
        }
        #endregion ProviderbyRegion

        public async Task<IActionResult> ViewNotes()
        {
            return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml");
        }
    }
}
