namespace RealEstateVehiclePlatform.WebUI.ViewModels.Message
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}