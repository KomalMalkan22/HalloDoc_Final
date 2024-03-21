using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
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

        #region ViewUpload
        public async Task<IActionResult> ViewUpload(int? id)
        {
            ViewUploadModel v = await _IActions.GetDocuments(id);
            return View("~/Views/AdminPanel/Actions/ViewUpload.cshtml", v);
        }
        #endregion ViewUpload

        #region UploadDocuments
        public IActionResult UploadDocuments(int Requestid, IFormFile file)
        {
            if (_IActions.UploadDocuments(Requestid, file))
            {
                _INotyfService.Success("File Uploaded Successfully.");
            }
            else
            {
                _INotyfService.Error("File not uploaded.");
            }
            return RedirectToAction("ViewUpload", "Actions", new { id = Requestid });
        }
        #endregion UploadDocuments

        #region DeleteFile
        public async Task<IActionResult> DeleteFile(int? id, int Requestid)
        {
            if (await _IActions.DeleteDocuments(id.ToString()))
            {
                _INotyfService.Success("File Deleted Successfully");
            }
            else
            {
                _INotyfService.Error("File Not Deleted");
            }
            return RedirectToAction("ViewUpload", "Actions", new { id = Requestid });
        }
        #endregion DeleteFile

        #region DeleteAllFiles
        public async Task<IActionResult> DeleteAllFiles(string deleteids, int Requestid)
        {
            if (await _IActions.DeleteDocuments(deleteids))
            {
                _INotyfService.Success("All Files are Deleted Successfully");
            }
            else
            {
                _INotyfService.Error("All Selected Files are Not Deleted");
            }
            return RedirectToAction("ViewUpload", "Actions", new { id = Requestid });
        }
        #endregion DeleteAllFiles

        #region SendOrder

        public async Task<IActionResult> Orders(int id)
        {
            List<ComboBoxHealthProfessionalType> hpt = await _IComboBox.ComboBoxHealthProfessionalType();
            ViewBag.ProfessionType = hpt;
            SendOrderModel data = new SendOrderModel
            {
                RequestId = id
            };
            return View("~/Views/AdminPanel/Actions/Orders.cshtml", data);
        }
        
        public Task<IActionResult> ProfessionByType(int HealthProfessionId)
        {
            var v = _IComboBox.ProfessionByType(HealthProfessionId);
            return Task.FromResult<IActionResult>(Json(v));
        }

        public Task<IActionResult> SelectProfessionalById(int VendorId)
        {
            var v = _IActions.SelectProfessionalById(VendorId);
            return Task.FromResult<IActionResult>(Json(v));
        }
        public IActionResult SendOrders(SendOrderModel som)
        {
            if (ModelState.IsValid)
            {
                bool data = _IActions.SendOrders(som);
                if (data)
                {
                    _INotyfService.Success("Order Created  successfully...");
                    //_INotyfService.Information("Mail is sended to Vendor successfully...");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _INotyfService.Error("Order Not Created...");
                    return View("../Actions/Orders", som);
                }
            }
            else
            {
                return View("../Actions/Orders", som);
            }
        }
        #endregion SendOrder

        #region SendAgreement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAgreementMail(int requestid)
        {
            if (_IActions.SendAgreement(requestid))
            {
                _INotyfService.Success("Mail Sent Successfully.");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region CloseCase
        public async Task<IActionResult> CloseCase(int RequestId)
        {
            CloseCaseModel model = _IActions.GetCloseCase(RequestId);
            return View("~/Views/AdminPanel/Actions/CloseCase.cshtml", model);
        }
        #endregion CloseCase

        #region EditCloseCase
        public async Task<IActionResult> EditCloseCase(CloseCaseModel model)
        {
            bool result = _IActions.EditCloseCase(model);
            if (result)
            {
                _INotyfService.Success("Case Edited Successfully.");
            }
            else
            {
                _INotyfService.Error("Case not Edited Successfully.");
            }
            return RedirectToAction("CloseCase", new {model.RequestId});
        }
        #endregion EditCloseCase

        #region CaseClosed
        public IActionResult CaseClosed(int id)
        {
            bool result = _IActions.CaseClosed(id);
            if (result)
            {
                _INotyfService.Success("Case Closed.");
                _INotyfService.Information("You can see Closed case in Unpaid State.");

            }
            else
            {
                _INotyfService.Error("Case Not Closed.");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion CaseClosed

        #region Encounter
        public IActionResult Encounter(int id)
        {
            EncounterModel model = _IActions.GetEncounterData(id);
            return View("~/Views/AdminPanel/Actions/Encounter.cshtml", model);
        }
        #endregion Encounter

        #region EditEncounterData
        public IActionResult EditEncounterData(EncounterModel model)
        {
            if (_IActions.EditEncounterData(model, CV.ID()))
            {
                _INotyfService.Success("Encounter Changes Saved.");
            }
            else
            {
                _INotyfService.Success("Encounter data remains unchanged.");
            }
            return RedirectToAction("Encounter", new { id = model.RequesId });
        }
        #endregion
    }
}