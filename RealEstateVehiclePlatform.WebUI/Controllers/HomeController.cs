using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.Home;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.Shared;
using System.Diagnostics;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ListingImageHelperService _listingImageHelperService;

        public HomeController(
            ApiService apiService,
            ListingImageHelperService listingImageHelperService)
        {
            _apiService = apiService;
            _listingImageHelperService = listingImageHelperService;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _apiService
                .GetListAsync<CategoryViewModel>("Categories");

            var listings = await _apiService
                .GetListAsync<ListingViewModel>("Listings");

            var latestListings = listings
                .OrderByDescending(x => x.Id)
                .Take(6)
                .ToList();

            await _listingImageHelperService
                .FillMainImagesAsync(latestListings);

            var model = new HomeViewModel
            {
                Categories = categories,
                LatestListings = latestListings
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}