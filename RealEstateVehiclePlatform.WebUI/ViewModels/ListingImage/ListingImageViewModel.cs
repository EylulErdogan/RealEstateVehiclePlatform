namespace RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage
{
    public class ListingImageViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMainImage { get; set; }

        public int DisplayOrder { get; set; }
    }
}