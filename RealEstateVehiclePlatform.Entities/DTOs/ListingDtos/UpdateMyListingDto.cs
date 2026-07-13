namespace RealEstateVehiclePlatform.Entities.DTOs.ListingDtos
{
    public class UpdateMyListingDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Address { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int ListingTypeId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }
    }
}