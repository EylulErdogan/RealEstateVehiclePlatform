using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminCategoryController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminCategoryController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {


            var values = await _apiService.GetListAsync<CategoryViewModel>("Categories");

            return View(values);
        }

        [HttpGet]
        public IActionResult Create()
        {


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            var result = await _apiService.PostAsync("Categories", model);

            if (!result)
            {
                TempData["Error"] = "Kategori eklenirken bir hata olustu.";
                return View(model);
            }

            TempData["Success"] = "Kategori basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {


            var value = await _apiService.GetAsync<UpdateCategoryViewModel>($"Categories/{id}");

            if (value == null)
            {
                TempData["Error"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateCategoryViewModel model)
        {
            var result = await _apiService.PutAsync("Categories", model);

            if (!result)
            {
                TempData["Error"] = "Kategori güncellenirken bir hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Kategori basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteAsync($"Categories/{id}");

            if (result)
                TempData["Success"] = "Kategori basariyla silindi.";
            else
                TempData["Error"] = "Kategori silinirken bir hata oluştu.";

            return RedirectToAction(nameof(Index));
        }
    }
}