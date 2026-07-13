using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IFavoriteService
    {
        bool ToggleFavorite(int userId, int listingId);

        bool IsFavorite(int userId, int listingId);

        List<Favorite> GetUserFavorites(int userId);

    }
}