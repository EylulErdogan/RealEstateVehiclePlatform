using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class ListingRepository : GenericRepository<Listing>, IListingRepository
    {
        public ListingRepository(AppDbContext context) : base(context)
        {
        }
    }
}