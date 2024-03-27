using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels;
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
            PaginationModel countRequest = _IAdminDashboard.CardData();
            return View("~/Views/AdminPanel/Dashboard/Index.cshtml", countRequest);
        }
        #endregion Index

        #region SearchResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> SearchResult(string Status, string Filter, PaginationModel pagination)
        {
            Status ??= CV.CurrentStatus();
            Filter ??= CV.Filter();
            Response.Cookies.Append("Status", Status);
            Response.Cookies.Append("Filter", Filter);

            PaginationModel contacts = _IAdminDashboard.GetRequests(Status, Filter, pagination);

            return Status switch
            {
                "1" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_NewRequest.cshtml", contacts)),
                "2" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_PendingRequest.cshtml", contacts)),
                "4,5" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_ActiveRequest.cshtml", contacts)),
                "6" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_ConcludeRequest.cshtml", contacts)),
                "3,7,8" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_ToCloseRequest.cshtml", contacts)),
                "9" => Task.FromResult<IActionResult>(PartialView("~/Views/AdminPanel/Dashboard/_UnpaidRequest.cshtml", contacts)),
                _ => Task.FromResult<IActionResult>(PartialView("")),
            };
        }
        #endregion SearchResult
    }
}
