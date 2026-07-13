using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage;

namespace RealEstateVehiclePlatform.WebUI.Services
{
    public class ListingImageHelperService
    {
        private readonly ApiService _apiService;

        public ListingImageHelperService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task FillMainImagesAsync(
            IEnumerable<ListingViewModel> listings)
        {
            foreach (var listing in listings)
            {
                var images = await _apiService
                    .GetListAsync<ListingImageViewModel>(
                        $"ListingImages/GetByListing/{listing.Id}");

                var mainImage = images
                    .OrderByDescending(x => x.IsMainImage)
                    .ThenBy(x => x.DisplayOrder)
                    .FirstOrDefault();

                listing.MainImageUrl = mainImage?.ImageUrl;
            }
        }
    }
}