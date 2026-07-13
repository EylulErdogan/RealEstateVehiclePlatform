using RealEstateVehiclePlatform.WebUI.ViewModels.Message;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Conversation
{
    public class ConversationDetailViewModel
    {
        public int ConversationId { get; set; }

        public int ListingId { get; set; }

        public int ReceiverId { get; set; }

        public int CurrentUserId { get; set; }

        public List<MessageViewModel> Messages { get; set; } = new();

        public SendMessageViewModel NewMessage { get; set; } = new();
    }
}