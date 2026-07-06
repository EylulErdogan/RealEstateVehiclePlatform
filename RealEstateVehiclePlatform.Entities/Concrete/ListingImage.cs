using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class ListingImage : BaseEntity
    {
        public int ListingId { get; set; }

        public Listing Listing { get; set; }

        public string ImageUrl { get; set; }

        public bool IsMainImage { get; set; } = false;

        public int DisplayOrder { get; set; }
    }
}