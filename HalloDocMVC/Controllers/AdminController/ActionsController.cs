using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
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
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region ProviderbyRegion
        public IActionResult ProviderbyRegion(int? Regionid)
        {
            var data = _IComboBox.ProviderByRegion(Regionid);
            return Json(data);
        }
        #endregion ProviderbyRegion

        #region CancelCase
        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {
            bool CancelCase = _IActions.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _INotyfService.Success("Case Cancelled Successfully");

            }
            else
            {
                _INotyfService.Error("Case Not Cancelled");

            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion CancelCase

        #region BlockCase
        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _IActions.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                _INotyfService.Success("Case Blocked Successfully");
            }
            else
            {
                _INotyfService.Error("Case Not Blocked");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion BlockCase

        #region TransferPhysician
        public async Task<IActionResult> TransferPhysician(int RequestId, int ProviderId, string Note)
        {
            if (await _IActions.TransferPhysician(RequestId, ProviderId, Note))
            {
                _INotyfService.Success("Physician Transferred Successfully.");
            }
            else
            {
                _INotyfService.Error("Physician Not Transferred.");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion TransferPhysician

        #region ClearCase
        public IActionResult ClearCase(int RequestID)
        {
            bool ClearCase = _IActions.ClearCase(RequestID);
            if(ClearCase)
            {
                _INotyfService.Success("Case Cleared Successfully.");
            }
            else
            {
                _INotyfService.Error("Case Not Cleared");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion ClearCase

        #region ViewNotes
        public IActionResult ViewNotes(int id)
        {
            ViewNotesModel vnm = _IActions.GetNotes(id);
            return View("~/Views/AdminPanel/Actions/ViewNotes.cshtml", vnm);
        }
        #endregion ViewNotes

        #region EditViewNotes
        public IActionResult EditViewNotes(string? AdminNotes, string? PhysicianNotes, int RequestId)
        {
            if(AdminNotes != null || PhysicianNotes != null)
            {
                bool result = _IActions.EditNotes(AdminNotes, PhysicianNotes, RequestId);
                if (result)
                {
                    _INotyfService.Success("Notes Updated Successfully.");
                    return RedirectToAction("ViewNotes", new { id = RequestId });
                }
                else
                {
                    _INotyfService.Error("Notes Not Updated.");
                    return View("../Actions/ViewNotes");
                }
            }
            else
            {
                _INotyfService.Information("Please Select one of the note!!");
                return RedirectToAction("ViewNotes", new { id = RequestId });
            }
        }
        #endregion EditViewNotes

        public async Task<IActionResult> ViewUpload()
        {
            return View("~/Views/AdminPanel/Actions/ViewUpload.cshtml");
        }
        public async Task<IActionResult> Orders()
        {
            return View("~/Views/AdminPanel/Actions/Orders.cshtml");
        }
    }
}
