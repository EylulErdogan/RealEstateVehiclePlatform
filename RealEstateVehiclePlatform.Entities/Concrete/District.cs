using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class District : BaseEntity
    {
        public string Name { get; set; }

        public int CityId { get; set; }

        public City? City { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}