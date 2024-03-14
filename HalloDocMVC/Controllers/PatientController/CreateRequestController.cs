using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Controllers.AdminController;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositeries.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.PatientController
{
    public class CreateRequestController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly ICreateRequest _ICreateRequest;
        private readonly INotyfService _INotyfService;
        public CreateRequestController(HalloDocContext context, ICreateRequest iActions, INotyfService iNotyfService)
        {
            _context = context;
            _ICreateRequest = iActions;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration
        public IActionResult Index()
        {
            return View("../PatientPanel/CreateRequest/SubmitRequest");
        }

        #region CheckEmail
        [HttpPost]
        public async Task<IActionResult> CheckEmailAsync(string email)
        {
            var aspnetuser = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == email);
            return Json(new
            {
                isAspnetuser = aspnetuser == null
            });
        }
        #endregion CheckEmail

        #region PatientRequest
        public IActionResult PatientRequest()
        {
            return View("../PatientPanel/CreateRequest/PatientRequest");
        }
        #endregion PatientRequest 

        #region CreatePatientRequest
        public async Task<IActionResult> CreatePatientRequest(CreatePatientRequestModel model)
        {
            if(await _ICreateRequest.PatientRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion CreatePatientRequest

        #region FamilyFriendRequest
        public IActionResult FamilyFriendRequest()
        {
            return View("../PatientPanel/CreateRequest/FamilyFriendRequest");
        }
        #endregion FamilyFriendRequest 

        #region CreateFamilyFriendRequest
        public async Task<IActionResult> CreateFamilyFriendRequest(CreateFamilyFriendRequestModel model)
        {
            if (await _ICreateRequest.FamilyFriendRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion CreateFamilyFriendRequest

        #region ConciergeRequest
        public IActionResult ConciergeRequest()
        {
            return View("../PatientPanel/CreateRequest/ConciergeRequest");
        }
        #endregion ConciergeRequest 

        #region CreateConciergeRequest
        public async Task<IActionResult> CreateConciergeRequest(CreateConciergeRequestModel model)
        {
            if (await _ICreateRequest.ConciergeRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion CreateConciergeRequest

        #region BusinessRequest
        public IActionResult BusinessRequest()
        {
            return View("../PatientPanel/CreateRequest/BusinessRequest");
        }
        #endregion BusinessRequest 

        #region CreateBusinessRequest
        public async Task<IActionResult> CreateBusinessRequest(CreateBusinessRequestModel model)
        {
            if (await _ICreateRequest.BusinessRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion CreateConciergeRequest
    }
}
