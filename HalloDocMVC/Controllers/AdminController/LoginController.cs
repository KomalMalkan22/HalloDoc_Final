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
        private readonly IActions _IActions;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;
        private readonly ILogin _ILogin;
        private readonly IJwtService _IJwtService;

        public LoginController(ILogger<ActionsController> logger,
                                      IComboBox ComboBox,
                                      IActions Actions,
                                      INotyfService NotyfService,
                                      ILogin Login,
                                      IJwtService JwtService)
        {
            _IComboBox = ComboBox;
            _IActions = Actions;
            _INotyfService = NotyfService;
            _logger = logger;
            _ILogin = Login;
            _IJwtService = JwtService;
        }
        #endregion Configuration

        public IActionResult Index()
        {
            return View("../../Views/AdminPanel/Login/Index");
        }
        public IActionResult ForgotPassword()
        {
            return View("../../Views/AdminPanel/Login/ForgotPassword");
        }
        public IActionResult ResetPassword()
        {
            return View("../../Views/AdminPanel/Login/ResetPassword");
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
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewData["error"] = "Invalid Id or Password";
                return View("../AdminPanel/Login/Index");
            }
        }
        #region Logout
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Login");
        }
        #endregion Logout
        public IActionResult AuthError()
        {
            return View("../AdminPanel/Login/AuthError");
        }
    }
}
