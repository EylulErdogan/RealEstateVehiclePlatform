using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Auth;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;
        private readonly LogService _logService; // LogService eklendi

        public AccountController(ApiService apiService, LogService logService)
        {
            _apiService = apiService;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _apiService.LoginAsync(model);

            if (result == null)
            {
                _logService.LogWarning($"Hatalı giriş denemesi. Kullanılan E-posta: {model.Email}");
                ViewBag.ErrorMessage = "Email veya şifre hatalı.";
                return View(model);
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("Email", result.Email);
            HttpContext.Session.SetString("Role", result.Role);

            _logService.LogInfo($"Başarılı kullanıcı girişi yapıldı.", result.FullName);

            if (result.Role == "Admin")
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Token") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestModel = new
            {
                model.FullName,
                model.UserName,
                model.Email,
                model.Password
            };

            var result = await _apiService.PostAsync(
                "Auth/Register",
                requestModel);

            if (!result)
            {
                _logService.LogWarning($"Kullanıcı kayıt işlemi başarısız. Kullanıcı Adı: {model.UserName}, E-posta: {model.Email}");
                ViewBag.ErrorMessage =
                    "Kayıt işlemi gerçekleştirilemedi. Email veya kullanıcı adı daha önce kullanılmış olabilir.";

                return View(model);
            }

            _logService.LogInfo($"Yeni kullanıcı kaydı oluşturuldu. Kullanıcı Adı: {model.UserName}", model.FullName);

            TempData["Success"] =
                "Kayıt işleminiz başarıyla tamamlandı. Giriş yapabilirsiniz.";

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            var fullName = HttpContext.Session.GetString("FullName");
            _logService.LogInfo("Oturum kapatıldı.", fullName);

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}