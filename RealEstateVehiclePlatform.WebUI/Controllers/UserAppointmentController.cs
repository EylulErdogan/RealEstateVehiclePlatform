using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Appointment;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserAppointmentController : Controller
    {
        private readonly ApiService _apiService;

        public UserAppointmentController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = await _apiService
                .GetListAsync<UserAppointmentViewModel>(
                    "Appointments/MyAppointments");

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int listingId)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var listing = await _apiService
                .GetAsync<ListingViewModel>($"Listings/{listingId}");

            if (listing == null)
            {
                TempData["Error"] = "İlan bulunamadı.";

                return RedirectToAction("Index", "Listing");
            }

            ViewBag.ListingTitle = listing.Title;
            ViewBag.ListingNo = listing.ListingNo;
            ViewBag.ListingPrice = listing.Price;
            ViewBag.ListingAddress = listing.Address;

            var model = new CreateAppointmentViewModel
            {
                ListingId = listingId,
                AppointmentDate = DateTime.Now.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateAppointmentViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.AppointmentDate <= DateTime.Now)
            {
                TempData["Error"] =
                    "Randevu tarihi geçmiş bir tarih olamaz.";

                return RedirectToAction(
                    nameof(Create),
                    new { listingId = model.ListingId });
            }

            var result = await _apiService
                .PostAsync("Appointments/Create", model);

            if (!result)
            {
                TempData["Error"] =
                    "Randevu oluşturulurken bir hata meydana geldi.";

                return RedirectToAction(
                    nameof(Create),
                    new { listingId = model.ListingId });
            }

            TempData["Success"] =
                "Randevu talebiniz başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _apiService
                .PutAsync<object>($"Appointments/Cancel/{id}", new { });

            if (result)
            {
                TempData["Success"] = "Randevu iptal edildi.";
            }
            else
            {
                TempData["Error"] =
                    "Randevu iptal edilirken bir hata meydana geldi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}