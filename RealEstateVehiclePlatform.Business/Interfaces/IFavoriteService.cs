using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IFavoriteService
    {
        void AddFavorite(int userId, int listingId);

        void RemoveFavorite(int userId, int listingId);

        List<Favorite> GetUserFavorites(int userId);
    }
}