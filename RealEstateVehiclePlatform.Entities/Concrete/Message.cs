using RealEstateVehiclePlatform.Entities.Abstract;

namespace RealEstateVehiclePlatform.Entities.Concrete
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }

        public Conversation Conversation { get; set; }

        public int SenderId { get; set; }

        public AppUser Sender { get; set; }

        public int ReceiverId { get; set; }

        public AppUser Receiver { get; set; }

        public string Content { get; set; }

        public bool IsRead { get; set; } = false;
    }
}