namespace RealEstateVehiclePlatform.WebUI.ViewModels.Listing
{
    public class ListingViewModel
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

        public int ListingTypeId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }

        public int UserId { get; set; }

        public bool IsFavorite { get; set; }

        public string? MainImageUrl { get; set; }
    }
}