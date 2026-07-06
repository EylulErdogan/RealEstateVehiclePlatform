using Microsoft.AspNetCore.Identity;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<Message> SentMessages { get; set; } = new List<Message>();

        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<Conversation> ConversationsAsUserOne { get; set; } = new List<Conversation>();

        public ICollection<Conversation> ConversationsAsUserTwo { get; set; } = new List<Conversation>();
    }
}