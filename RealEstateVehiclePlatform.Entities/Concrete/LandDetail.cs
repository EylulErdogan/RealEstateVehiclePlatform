using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class LandDetail : BaseEntity
    {
        public int ListingId { get; set; }

        public Listing Listing { get; set; }

        public int SquareMeters { get; set; }

        public string ZoningStatus { get; set; }

        public string BlockNo { get; set; }

        public string ParcelNo { get; set; }

        public string SheetNo { get; set; }

        public bool IsSuitableForCredit { get; set; }
    }
}