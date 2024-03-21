using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Models;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class MyProfileController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IMyProfile _IMyProfile;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;

        public MyProfileController(HalloDocContext context, IMyProfile iMyProfile, IComboBox iComboBox, INotyfService iNotyfService)
        {
            _context = context;
            _IMyProfile = iMyProfile;
            _IComboBox = iComboBox;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration

        #region Index
        public async Task<IActionResult> Index()
        {
            AdminProfile data = await _IMyProfile.GetProfile(Convert.ToInt32(CV.UserID()));
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxUserRole = await _IComboBox.ComboBoxUserRole();
            return View("../AdminPanel/MyProfile/Index", data);
        }
        #endregion Index

        #region ResetPassword
        public async Task<IActionResult> ResetPassword(string Password)
        {
            if (await _IMyProfile.ResetPassword(Password, Convert.ToInt32(CV.UserID())))
            {
                _INotyfService.Success("Password changed Successfully.");
            }
            else
            {
                _INotyfService.Error("Password remains unchanged");
            }
            return RedirectToAction("Index");
        }
        #endregion ResetPassword

        #region EditAdministratorInfo
        [HttpPost]  
        public async Task<IActionResult> EditAdministratorInfo(AdminProfile profile)
        {
            if (await _IMyProfile.EditAdministratorInformation(profile))
            {
                _INotyfService.Success("Information changed Successfully.");
            }
            else
            {
                _INotyfService.Error("Information remains unchanged.");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> EditBillingInfo(AdminProfile profile)
        {
            if (await _IMyProfile.EditBillingInformation(profile))
            {
                _INotyfService.Success("Information changed Successfully.");
            }
            else
            {
                _INotyfService.Error("Information remains unchanged.");
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
