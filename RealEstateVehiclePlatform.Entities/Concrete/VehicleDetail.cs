using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class VehicleDetail : BaseEntity
    {
        public int ListingId { get; set; }

        public Listing Listing { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public int Kilometer { get; set; }

        public string FuelType { get; set; }

        public string GearType { get; set; }

        public string BodyType { get; set; }

        public string Color { get; set; }
    }
}