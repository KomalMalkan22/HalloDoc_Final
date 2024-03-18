using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Models;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.AdminController
{
    public class DashboardController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IAdminDashboard _IAdminDashboard;
        private readonly IComboBox _IComboBox;
        private readonly ILogger<DashboardController> _Logger;
        public DashboardController(HalloDocContext context, IAdminDashboard iAdminDashboard, IComboBox iComboBox)
        {
            _context = context;
            _IAdminDashboard = iAdminDashboard;
            _IComboBox = iComboBox;
        }
        #endregion Configuration

        [CheckProviderAccess("Admin")]
        #region Index
        public async Task<IActionResult> Index()
        {
            ViewBag.ComboBoxRegion = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBox.ComboBoxCaseReasons();
            var countRequest = _IAdminDashboard.CardData();
            return View("~/Views/AdminPanel/Dashboard/Index.cshtml", countRequest);
        }
        #endregion Index

        #region SearchResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchResult(string Status)
        {
            Status ??= CV.CurrentStatus();
            Response.Cookies.Append("Status", Status);

            List<AdminDashboardList> contacts = _IAdminDashboard.GetRequests(Status);
            
            switch (Status)
            {
                case "1":
                    return PartialView("~/Views/AdminPanel/Dashboard/_NewRequest.cshtml", contacts);
                    break;
                case "2":
                    return PartialView("~/Views/AdminPanel/Dashboard/_PendingRequest.cshtml", contacts);
                    break;
                case "4,5":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ActiveRequest.cshtml", contacts);
                    break;
                case "6":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ConcludeRequest.cshtml", contacts);
                    break;
                case "3,7,8":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ToCloseRequest.cshtml", contacts);
                    break;
                case "9":
                    return PartialView("~/Views/AdminPanel/Dashboard/_UnpaidRequest.cshtml", contacts);
                    break;
            }

      
            return PartialView("");
        }
        #endregion SearchResult


    }
}
