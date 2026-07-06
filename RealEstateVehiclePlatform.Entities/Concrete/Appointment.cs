using RealEstateVehiclePlatform.Entities.Abstract;
using RealEstateVehiclePlatform.Entities.Enums;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Appointment : BaseEntity
    {
        public int UserId { get; set; }

        public AppUser User { get; set; }

        public int ListingId { get; set; }

        public Listing Listing { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }
}