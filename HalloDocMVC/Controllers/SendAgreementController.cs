using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class SendAgreementController : Controller
    {
        private readonly IActions _IActions;
        private readonly INotyfService _INotyfService;
        public SendAgreementController(IActions Actions, INotyfService NotyfService)
        {
            _IActions = Actions;
            _INotyfService = NotyfService;
        }
        public IActionResult Index(int RequestId)
        {
            TempData["RequestID"] = " " + RequestId;
            TempData["PatientName"] = "Komal Malkan";

            return View();
        }
        public IActionResult accept(int RequestId)
        {
            _IActions.SendAgreement_Accept(RequestId);
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Reject(int RequestId, string Notes)
        {
            _IActions.SendAgreement_Reject(RequestId, Notes);
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
