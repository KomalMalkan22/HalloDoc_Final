using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.AdminController
{
    public class DashboardController : Controller
    {
        private readonly HalloDocContext _context;
        private readonly IAdminDashboard _IAdminDashboard;
        public DashboardController(HalloDocContext context, IAdminDashboard IAdminDashboard)
        {
            _context = context;
            _IAdminDashboard = IAdminDashboard;
        }
        public IActionResult Index()
        {
            var countRequest = _IAdminDashboard.CardData();
            return View("~/Views/AdminPanel/Dashboard/Index.cshtml", countRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(string Status)
        {
            if (Status == null)
            {
                Status = "1";
            }

            List<AdminDashboardList> contacts = _IAdminDashboard.GetRequests(Status);
            switch (Status)
            {
                case "1":
                    TempData["CurrentStatus"] = "New";
                    break;
                case "2":
                    TempData["CurrentStatus"] = "Pending";
                    break;
                case "4,5":
                    TempData["CurrentStatus"] = "Active";
                    break;
                case "6":
                    TempData["CurrentStatus"] = "Conclude";
                    break;
                case "3,7,8":
                    TempData["CurrentStatus"] = "To Close";
                    break;
                case "9":
                    TempData["CurrentStatus"] = "Unpaid";
                    break;
            }
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
    }
}
