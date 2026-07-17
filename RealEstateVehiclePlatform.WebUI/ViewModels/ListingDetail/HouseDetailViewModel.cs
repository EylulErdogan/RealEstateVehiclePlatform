namespace RealEstateVehiclePlatform.WebUI.ViewModels.ListingDetail
{
    public class HouseDetailViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public int? RoomCount { get; set; }

        public int? LivingRoomCount { get; set; }

        public int? GrossSquareMeters { get; set; }

        public int? NetSquareMeters { get; set; }

        public int? BuildingAge { get; set; }

        public int? FloorNumber { get; set; }

        public int? TotalFloors { get; set; }

        public string? HeatingType { get; set; }

        public bool HasBalcony { get; set; }

        public bool IsFurnished { get; set; }
    }
}