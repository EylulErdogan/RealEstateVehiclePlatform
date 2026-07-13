using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiService _apiService;

        public CategoryController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService.GetListAsync<CategoryViewModel>("Categories");
            return View(values);
        }
    }
}