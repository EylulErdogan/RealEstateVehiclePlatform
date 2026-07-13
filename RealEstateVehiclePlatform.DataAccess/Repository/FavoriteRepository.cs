using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Favorite? GetByUserAndListing(int userId, int listingId)
        {
            return _context.Favorites.FirstOrDefault(x =>
                x.UserId == userId &&
                x.ListingId == listingId &&
                !x.IsDeleted);
        }
    }
}