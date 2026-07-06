using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Conversation : BaseEntity
    {
        public int? ListingId { get; set; }

        public Listing Listing { get; set; }

        public int UserOneId { get; set; }

        public AppUser UserOne { get; set; }

        public int UserTwoId { get; set; }

        public AppUser UserTwo { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}