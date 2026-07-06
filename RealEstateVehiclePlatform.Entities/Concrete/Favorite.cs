using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Favorite : BaseEntity
    {
        public int UserId { get; set; }

        public AppUser User { get; set; }

        public int ListingId { get; set; }

        public Listing Listing { get; set; }
    }
}