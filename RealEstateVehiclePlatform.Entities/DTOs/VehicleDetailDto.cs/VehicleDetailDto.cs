namespace RealEstateVehiclePlatform.Entities.DTOs.VehicleDetailDtos
{
    public class VehicleDetailDto
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public int Kilometer { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public string GearType { get; set; } = string.Empty;

        public string BodyType { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;
    }
}