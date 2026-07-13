namespace RealEstateVehiclePlatform.WebUI.ViewModels.Conversation
{
    public class ConversationViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public int UserOneId { get; set; }

        public int UserTwoId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}