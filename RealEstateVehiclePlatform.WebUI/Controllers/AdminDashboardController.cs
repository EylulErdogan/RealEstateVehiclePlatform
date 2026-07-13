using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Dashboard;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApiService _apiService;

        public AdminDashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var summary = await _apiService.GetAsync<AdminDashboardSummaryViewModel>("AdminDashboard/Summary");

            if (summary == null)
            {
                summary = new AdminDashboardSummaryViewModel();
            }

            return View(summary);
        }
    }
}