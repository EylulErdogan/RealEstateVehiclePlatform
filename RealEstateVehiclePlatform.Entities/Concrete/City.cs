using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class City : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}