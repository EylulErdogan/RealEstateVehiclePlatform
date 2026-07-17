using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.Dashboard;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _env;

        public AdminDashboardController(ApiService apiService, IWebHostEnvironment env)
        {
            _apiService = apiService;
            _env = env;
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

        // Gelişmiş İstatistikler Aksiyonu (YENİ EKLENDİ)
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var listings = await _apiService.GetListAsync<ListingViewModel>("Listings/AdminAll");

            var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
            var categoryMap = categories.ToDictionary(x => x.Id, x => x.Name);

            var cities = await _apiService.GetListAsync<CityViewModel>("Cities");
            var cityMap = cities.ToDictionary(x => x.Id, x => x.Name);

            // 1. En çok görüntülenen popüler 5 ilan
            var topListings = listings
                .OrderByDescending(x => x.ViewCount)
                .Take(5)
                .ToList();

            // 2. Kategorilere göre gruplayarak ilan sayıları
            var categoryStats = listings
                .GroupBy(x => x.CategoryId)
                .Select(g => new
                {
                    CategoryName = categoryMap.ContainsKey(g.Key) ? categoryMap[g.Key] : "Belirtilmemiş",
                    Count = g.Count()
                })
                .ToDictionary(x => x.CategoryName, x => x.Count);

            // 3. Şehirlere göre gruplayarak ilan sayıları
            var cityStats = listings
                .GroupBy(x => x.CityId)
                .Select(g => new
                {
                    CityName = cityMap.ContainsKey(g.Key) ? cityMap[g.Key] : "Belirtilmemiş",
                    Count = g.Count()
                })
                .ToDictionary(x => x.CityName, x => x.Count);

            ViewBag.TopListings = topListings;
            ViewBag.CategoryStats = categoryStats;
            ViewBag.CityStats = cityStats;
            ViewBag.CategoryMap = categoryMap;
            ViewBag.CityMap = cityMap;

            return View();
        }

        [HttpGet]
        public IActionResult Logs()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var logPath = Path.Combine(_env.WebRootPath, "logs", "system_logs.txt");
            var logsList = new List<string>();

            if (System.IO.File.Exists(logPath))
            {
                logsList = System.IO.File.ReadAllLines(logPath).Reverse().ToList();
            }

            return View(logsList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearLogs()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Forbid();
            }

            var logPath = Path.Combine(_env.WebRootPath, "logs", "system_logs.txt");
            if (System.IO.File.Exists(logPath))
            {
                System.IO.File.WriteAllText(logPath, string.Empty);
            }

            TempData["Success"] = "Sistem günlükleri başarıyla temizlendi.";
            return RedirectToAction(nameof(Logs));
        }
    }
}