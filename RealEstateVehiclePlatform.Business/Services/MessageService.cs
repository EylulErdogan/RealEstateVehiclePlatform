using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void SendMessage(Message message)
        {
            var conversation = _unitOfWork.Conversations.GetById(message.ConversationId);

            if (conversation == null)
                throw new Exception("Konuşma bulunamadı.");

            if (message.SenderId == message.ReceiverId)
                throw new Exception("Kullanıcı kendisine mesaj gönderemez.");

            if (string.IsNullOrWhiteSpace(message.Content))
                throw new Exception("Mesaj içeriği boş olamaz.");

            _unitOfWork.Messages.Insert(message);
            _unitOfWork.Save();
        }

        public List<Message> GetConversationMessages(int conversationId)
        {
            return _unitOfWork.Messages
                .GetAll()
                .Where(x => x.ConversationId == conversationId)
                .OrderBy(x => x.CreatedDate)
                .ToList();
        }

        public void MarkAsRead(int messageId)
        {
            var message = _unitOfWork.Messages.GetById(messageId);

            if (message == null)
                throw new Exception("Mesaj bulunamadı.");

            message.IsRead = true;

            _unitOfWork.Messages.Update(message);
            _unitOfWork.Save();
        }
    }
}