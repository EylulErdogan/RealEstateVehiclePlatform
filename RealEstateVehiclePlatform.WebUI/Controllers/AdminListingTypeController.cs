using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingType;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminListingTypeController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminListingTypeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService.GetListAsync<ListingTypeViewModel>("ListingTypes");
            return View(values);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateListingTypeViewModel model)
        {
            var result = await _apiService.PostAsync("ListingTypes", model);

            if (!result)
            {
                TempData["Error"] = "Ilan tipi eklenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Ilan tipi basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var value = await _apiService.GetAsync<UpdateListingTypeViewModel>($"ListingTypes/{id}");

            if (value == null)
            {
                TempData["Error"] = "Ilan tipi bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateListingTypeViewModel model)
        {
            var result = await _apiService.PutAsync("ListingTypes", model);

            if (!result)
            {
                TempData["Error"] = "Ilan tipi güncellenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Ilan tipi basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteAsync($"ListingTypes/{id}");

            if (result)
                TempData["Success"] = "Ilan tipi basariyla silindi.";
            else
                TempData["Error"] = "Ilan tipi silinirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }
    }
}