using RealEstateVehiclePlatform.Entities.Abstract;
using RealEstateVehiclePlatform.Entities.Enums;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Listing : BaseEntity
    {
        // Temel Bilgiler
        public string Title { get; set; } = string.Empty;

        public string ListingNo { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Address { get; set; } = string.Empty;

        public int ViewCount { get; set; } = 0;

        public ListingStatus Status { get; set; } = ListingStatus.Pending;

        // Foreign Keys
        public int CategoryId { get; set; }

        public int ListingTypeId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }

        public int UserId { get; set; }

        // Navigation Properties
        public Category? Category { get; set; }

        public ListingType? ListingType { get; set; }

        public City? City { get; set; }

        public District? District { get; set; }

        public AppUser? User { get; set; }

        // Child Tables
        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public HouseDetail? HouseDetail { get; set; }

        public LandDetail? LandDetail { get; set; }

        public VehicleDetail? VehicleDetail { get; set; }
    }
}