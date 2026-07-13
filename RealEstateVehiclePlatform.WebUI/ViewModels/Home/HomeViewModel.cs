using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = new();

        public List<ListingViewModel> LatestListings { get; set; } = new();
    }
}