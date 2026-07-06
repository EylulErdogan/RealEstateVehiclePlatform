using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class ListingType : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}