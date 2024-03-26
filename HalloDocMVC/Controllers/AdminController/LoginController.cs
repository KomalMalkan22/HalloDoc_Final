using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace HalloDocMVC.Controllers.AdminController
{
    public class LoginController : Controller
    {
        #region Configuration
        private readonly ILogin _ILogin;
        private readonly IJwtService _IJwtService;

        public LoginController(ILogin Login, IJwtService JwtService)
        {
            _ILogin = Login;
            _IJwtService = JwtService;
        }
        #endregion Configuration

        public IActionResult Index()
        {
            return View("../AdminPanel/Login/Index");
        }
        public IActionResult LandingPage()
        {
            return View("../AdminPanel/Login/LandingPage");
        }
        public IActionResult ForgotPassword()
        {
            return View("../AdminPanel/Login/ForgotPassword");
        }
        public IActionResult ResetPassword()
        {
            return View("../AdminPanel/Login/ResetPassword");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(Aspnetuser aspNetUser)
        {
            UserInformation u = await _ILogin.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _IJwtService.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Status", "1");
                Response.Cookies.Append("Filter", "1,2,3,4");
                if (u.Role == "Patient")
                {
                    return RedirectToAction("Index", "PatientDashboard");
                }
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewData["error"] = "Invalid Id or Password";
                return View("../AdminPanel/Login/Index");
            }
        }
        #region Logout
        public Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("Status");
            Response.Cookies.Delete("Filter");
            return Task.FromResult<IActionResult>(RedirectToAction("Index", "Login"));
        }
        #endregion Logout
        public IActionResult AuthError()
        {
            return View("../AdminPanel/Login/AuthError");
        }
    }
}
