using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConversationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Conversation StartConversation(int listingId, int userOneId, int userTwoId)
        {
            var listing = _unitOfWork.Listings.GetById(listingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            if (userOneId == userTwoId)
                throw new Exception("Kullanıcı kendisiyle konuşma başlatamaz.");

            var existingConversation = _unitOfWork.Conversations.GetByFilter(x =>
                x.ListingId == listingId &&
                !x.IsDeleted &&
                (
                    (x.UserOneId == userOneId && x.UserTwoId == userTwoId) ||
                    (x.UserOneId == userTwoId && x.UserTwoId == userOneId)
                ));

            if (existingConversation != null)
                return existingConversation;

            var conversation = new Conversation
            {
                ListingId = listingId,
                UserOneId = userOneId,
                UserTwoId = userTwoId
            };

            _unitOfWork.Conversations.Insert(conversation);
            _unitOfWork.Save();

            return conversation;
        }

        public List<Conversation> GetUserConversations(int userId)
        {
            return _unitOfWork.Conversations
                .GetAll()
                .Where(x => x.UserOneId == userId || x.UserTwoId == userId)
                .ToList();
        }

        public Conversation GetById(int id)
        {
            return _unitOfWork.Conversations.GetById(id);
        }
    }
}