using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.District;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminDistrictController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminDistrictController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService.GetListAsync<DistrictViewModel>("Districts");
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateDistrictViewModel
            {
                Cities = await GetCitySelectList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDistrictViewModel model)
        {
            var result = await _apiService.PostAsync("Districts", model);

            if (!result)
            {
                model.Cities = await GetCitySelectList();

                TempData["Error"] = "İlçe eklenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "İlçe basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var value = await _apiService.GetAsync<UpdateDistrictViewModel>($"Districts/{id}");

            if (value == null)
            {
                TempData["Error"] = "İlçe bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            value.Cities = await GetCitySelectList();

            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateDistrictViewModel model)
        {
            var result = await _apiService.PutAsync("Districts", model);

            if (!result)
            {
                model.Cities = await GetCitySelectList();

                TempData["Error"] = "İlçe güncellenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "İlçe basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteAsync($"Districts/{id}");

            if (result)
                TempData["Success"] = "İlçe basariyla silindi.";
            else
                TempData["Error"] = "İlçe silinirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetCitySelectList()
        {
            var cities = await _apiService.GetListAsync<CityViewModel>("Cities");

            return cities.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }
    }
}