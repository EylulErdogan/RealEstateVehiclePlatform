namespace RealEstateVehiclePlatform.WebUI.ViewModels.ListingDetail
{
    public class VehicleDetailViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int? Year { get; set; }

        public int? Kilometer { get; set; }

        public string? FuelType { get; set; }

        public string? GearType { get; set; }

        public string? BodyType { get; set; }

        public string? Color { get; set; }

        public int? EnginePower { get; set; }

        public int? EngineCapacity { get; set; }
    }
}