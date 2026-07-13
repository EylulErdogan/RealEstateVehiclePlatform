namespace RealEstateVehiclePlatform.Entities.DTOs.ListingImageDtos
{
    public class CreateListingImageDto
    {
        public int ListingId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMainImage { get; set; }

        public int DisplayOrder { get; set; }
    }
}