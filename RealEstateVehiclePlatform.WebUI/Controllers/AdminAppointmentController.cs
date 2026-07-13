using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Appointment;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminAppointmentController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminAppointmentController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService.GetListAsync<AppointmentViewModel>("Appointments");
            return View(values);
        }

        public async Task<IActionResult> Approve(int id)
        {
            var result = await _apiService.PutAsync<object>($"Appointments/Approve/{id}", new { });

            TempData[result ? "Success" : "Error"] =
                result ? "Randevu onaylandı." : "Randevu onaylanırken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            var result = await _apiService.PutAsync<object>($"Appointments/Reject/{id}", new { });

            TempData[result ? "Success" : "Error"] =
                result ? "Randevu reddedildi." : "Randevu reddedilirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Complete(int id)
        {
            var result = await _apiService.PutAsync<object>($"Appointments/Complete/{id}", new { });

            TempData[result ? "Success" : "Error"] =
                result ? "Randevu tamamlandı." : "Randevu tamamlanırken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _apiService.PutAsync<object>($"Appointments/Cancel/{id}", new { });

            TempData[result ? "Success" : "Error"] =
                result ? "Randevu iptal edildi." : "Randevu iptal edilirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }
    }
}