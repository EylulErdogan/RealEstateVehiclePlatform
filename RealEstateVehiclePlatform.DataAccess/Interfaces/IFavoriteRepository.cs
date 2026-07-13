using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Favorite? GetByUserAndListing(int userId, int listingId);
    }
}