using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Favorite;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserFavoriteController : Controller
    {
        private readonly ApiService _apiService;

        public UserFavoriteController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favorites = await _apiService
                .GetListAsync<FavoriteViewModel>("Favorites/MyFavorites");

            var listings = await _apiService
                .GetListAsync<ListingViewModel>("Listings");

            var model = favorites
                .Join(
                    listings,
                    favorite => favorite.ListingId,
                    listing => listing.Id,
                    (favorite, listing) => new FavoriteListItemViewModel
                    {
                        FavoriteId = favorite.Id,
                        ListingId = listing.Id,
                        ListingNo = listing.ListingNo,
                        Title = listing.Title,
                        Price = listing.Price,
                        Address = listing.Address,
                        ViewCount = listing.ViewCount,
                        CreatedDate = favorite.CreatedDate
                    })
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int listingId)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return Json(new
                {
                    success = false,
                    redirectUrl = "/Account/Login"
                });
            }

            var result = await _apiService
                .PostAndGetAsync<object, FavoriteToggleResponseViewModel>(
                    $"Favorites/Toggle/{listingId}",
                    new { });

            if (result == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Favori işlemi sırasında hata oluştu."
                });
            }

            return Json(new
            {
                success = true,
                isFavorite = result.IsFavorite,
                message = result.Message
            });
        }

        [HttpGet]
        public async Task<IActionResult> IsFavorite(int listingId)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return Json(new
                {
                    success = true,
                    isFavorite = false
                });
            }

            var result = await _apiService
                .GetAsync<FavoriteStatusViewModel>(
                    $"Favorites/IsFavorite/{listingId}");

            return Json(new
            {
                success = result != null,
                isFavorite = result?.IsFavorite ?? false
            });
        }
    }
}