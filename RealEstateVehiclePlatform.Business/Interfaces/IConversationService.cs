using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IConversationService
    {
        Conversation StartConversation(int listingId, int userOneId, int userTwoId);

        List<Conversation> GetUserConversations(int userId);

        Conversation GetById(int id);
    }
}