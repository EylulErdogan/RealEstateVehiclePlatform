using RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Listing
{
    public class ListingDetailViewModel
    {
        public int Id { get; set; }

        public string ListingNo { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Address { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public int Status { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int ListingTypeId { get; set; }

        public string ListingTypeName { get; set; } = string.Empty;

        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;

        public int DistrictId { get; set; }

        public string DistrictName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string OwnerFullName { get; set; } = string.Empty;

        public List<ListingImageViewModel> Images { get; set; } = new();
    }
}