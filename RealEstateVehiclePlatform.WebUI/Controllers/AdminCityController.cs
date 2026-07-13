using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminCityController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminCityController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService.GetListAsync<CityViewModel>("Cities");
            return View(values);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCityViewModel model)
        {
            var result = await _apiService.PostAsync("Cities", model);

            if (!result)
            {
                TempData["Error"] = "Şehir eklenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Şehir basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var value = await _apiService.GetAsync<UpdateCityViewModel>($"Cities/{id}");

            if (value == null)
            {
                TempData["Error"] = "Şehir bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCityViewModel model)
        {
            var result = await _apiService.PutAsync("Cities", model);

            if (!result)
            {
                TempData["Error"] = "Şehir güncellenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Şehir basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteAsync($"Cities/{id}");

            if (result)
                TempData["Success"] = "Şehir basariyla silindi.";
            else
                TempData["Error"] = "Şehir silinirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }
    }
}