using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Profile;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ApiService _apiService;

        public UserProfileController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _apiService
                .GetAsync<UserProfileViewModel>(
                    "Profile/MyProfile");

            if (profile == null)
            {
                TempData["Error"] = "Profil bilgileri alınamadı.";

                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            UpdateUserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] =
                    "Lütfen profil bilgilerini kontrol ediniz.";

                return RedirectToAction(nameof(Index));
            }

            var result = await _apiService.PutAsync(
                "Profile/Update",
                model);

            if (!result)
            {
                TempData["Error"] =
                    "Profil güncellenirken bir hata oluştu.";

                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetString(
                "FullName",
                model.FullName);

            TempData["Success"] =
                "Profil bilgileriniz başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestModel = new
            {
                model.CurrentPassword,
                model.NewPassword
            };

            var result = await _apiService.PutAsync(
                "Profile/ChangePassword",
                requestModel);

            if (!result)
            {
                ViewBag.ErrorMessage =
                    "Şifre değiştirilemedi. Mevcut şifrenizi kontrol ediniz.";

                return View(model);
            }

            TempData["Success"] =
                "Şifreniz başarıyla değiştirildi. Yeniden giriş yapınız.";

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}