namespace RealEstateVehiclePlatform.Entities.DTOs.LandDetailDtos
{
    public class LandDetailDto
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public int SquareMeters { get; set; }

        public string ZoningStatus { get; set; } = string.Empty;

        public string BlockNo { get; set; } = string.Empty;

        public string ParcelNo { get; set; } = string.Empty;

        public string SheetNo { get; set; } = string.Empty;

        public bool IsSuitableForCredit { get; set; }
    }
}