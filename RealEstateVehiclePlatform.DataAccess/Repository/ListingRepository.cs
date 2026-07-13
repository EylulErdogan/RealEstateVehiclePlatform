using Microsoft.EntityFrameworkCore;
using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class ListingRepository : GenericRepository<Listing>, IListingRepository
    {
        private readonly AppDbContext _context;

        public ListingRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }
        public Listing? GetByIdWithDetails(int id)
        {
            return _context.Listings
                .Include(x => x.Category)
                .Include(x => x.ListingType)
                .Include(x => x.City)
                .Include(x => x.District)
                .Include(x => x.User)
                .FirstOrDefault(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }
    }
}