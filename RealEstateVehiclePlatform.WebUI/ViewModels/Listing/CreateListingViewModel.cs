using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Listing
{
    public class CreateListingViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string ListingNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Address { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public int ListingTypeId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? ListingTypes { get; set; }
        public List<SelectListItem>? Cities { get; set; }
        public List<SelectListItem>? Districts { get; set; }
    }
}