using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.Favorite;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class ListingController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ListingImageHelperService _listingImageHelperService;

        public ListingController(
            ApiService apiService,
            ListingImageHelperService listingImageHelperService)
        {
            _apiService = apiService;
            _listingImageHelperService = listingImageHelperService;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? category,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var listings = await _apiService
                .GetListAsync<ListingViewModel>("Listings");

            var categories = await _apiService
                .GetListAsync<CategoryViewModel>("Categories");

            if (!string.IsNullOrWhiteSpace(search))
            {
                listings = listings
                    .Where(x =>
                        x.Title.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||
                        x.Address.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryId = categories
                    .FirstOrDefault(x =>
                        x.Name.Equals(
                            category,
                            StringComparison.OrdinalIgnoreCase))
                    ?.Id;

                if (categoryId.HasValue)
                {
                    listings = listings
                        .Where(x => x.CategoryId == categoryId.Value)
                        .ToList();
                }
            }

            if (minPrice.HasValue)
            {
                listings = listings
                    .Where(x => x.Price >= minPrice.Value)
                    .ToList();
            }

            if (maxPrice.HasValue)
            {
                listings = listings
                    .Where(x => x.Price <= maxPrice.Value)
                    .ToList();
            }

            if (HttpContext.Session.GetString("Token") != null)
            {
                var favorites = await _apiService
                    .GetListAsync<FavoriteViewModel>(
                        "Favorites/MyFavorites");

                var favoriteListingIds = favorites
                    .Select(x => x.ListingId)
                    .ToHashSet();

                foreach (var listing in listings)
                {
                    listing.IsFavorite =
                        favoriteListingIds.Contains(listing.Id);
                }
            }

            await _listingImageHelperService
                .FillMainImagesAsync(listings);

            return View(listings);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var listing = await _apiService
                .GetAsync<ListingDetailViewModel>(
                    $"Listings/{id}");

            if (listing == null)
            {
                TempData["Error"] = "İlan bulunamadı.";

                return RedirectToAction(nameof(Index));
            }

            var images = await _apiService
                .GetListAsync<ListingImageViewModel>(
                    $"ListingImages/GetByListing/{id}");

            listing.Images = images;

            return View(listing);
        }
    }
}