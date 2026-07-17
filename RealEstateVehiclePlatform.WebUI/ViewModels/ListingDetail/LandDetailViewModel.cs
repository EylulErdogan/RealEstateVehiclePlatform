namespace RealEstateVehiclePlatform.WebUI.ViewModels.ListingDetail
{
    public class LandDetailViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public int? SquareMeters { get; set; }

        public string? ZoningStatus { get; set; }

        public string? BlockNo { get; set; }

        public string? ParcelNo { get; set; }

        public string? SheetNo { get; set; }

        public bool IsSuitableForCredit { get; set; }
    }
}