using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Models;
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
        public MyProfileController(HalloDocContext context, IMyProfile iMyProfile, IComboBox iComboBox)
        {
            _context = context;
            _IMyProfile = iMyProfile;
            _IComboBox = iComboBox;
        }
        #endregion Configuration
        public async Task<IActionResult> Index()
        {
            AdminProfile data = await _IMyProfile.GetProfile((int)CV.UserID());
            return View("../AdminPanel/MyProfile/Index", data);
        }
    }
}
