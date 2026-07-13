namespace RealEstateVehiclePlatform.WebUI.ViewModels.Message
{
    public class SendMessageViewModel
    {
        public int ConversationId { get; set; }

        public int ReceiverId { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}