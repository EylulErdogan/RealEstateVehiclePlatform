using RealEstateVehiclePlatform.Entities.Abstract;
using RealEstateVehiclePlatform.Entities.Enums;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Listing : BaseEntity
    {
        public string Title { get; set; }
        public string ListingNo { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Address { get; set; }

        public int ViewCount { get; set; } = 0;

        public ListingStatus Status { get; set; } = ListingStatus.Pending;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int ListingTypeId { get; set; }
        public ListingType ListingType { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; }

        public int UserId { get; set; }
        public AppUser User { get; set; }

        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public HouseDetail HouseDetail { get; set; }

        public LandDetail LandDetail { get; set; }

        public VehicleDetail VehicleDetail { get; set; }
    }
}