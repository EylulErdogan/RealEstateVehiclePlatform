using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IMessageService
    {
        void SendMessage(Message message);

        List<Message> GetConversationMessages(int conversationId);

        void MarkAsRead(int messageId);
    }
}