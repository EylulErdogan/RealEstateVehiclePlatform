using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}