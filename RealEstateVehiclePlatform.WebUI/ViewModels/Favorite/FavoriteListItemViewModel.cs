namespace RealEstateVehiclePlatform.WebUI.ViewModels.Favorite
{
    public class FavoriteListItemViewModel
    {
        public int FavoriteId { get; set; }

        public int ListingId { get; set; }

        public string ListingNo { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Address { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}