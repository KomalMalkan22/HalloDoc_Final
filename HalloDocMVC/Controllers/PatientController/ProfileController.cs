using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositeries.Patient.Repository;
using HalloDocMVC.Repositeries.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.PatientController
{
    public class ProfileController : Controller
    {
        #region Configuration
        private readonly IProfile _IProfile;
        private readonly INotyfService _INotyfService;
        public ProfileController(IProfile iProfile, INotyfService iNotyfService)
        {
            _IProfile = iProfile;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration

        #region GetProfile
        public IActionResult Index()
        {
            UserProfileModel model = _IProfile.GetProfile();
            return View("../PatientPanel/Profile/Index", model);
        }
        #endregion GetProfile

        #region EditProfile
        public async Task<IActionResult> EditProfile(UserProfileModel model)
        {
            if(await _IProfile.EditProfile(model))
            {
                _INotyfService.Success("Profile has been edited successfully.");
            }
            else
            {
                _INotyfService.Error("Profile has been edited successfully.");
            }
            return RedirectToAction("Index", "Profile");
        }
        #endregion EditProfile
    }
}
