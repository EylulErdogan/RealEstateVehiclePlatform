using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class ListingImageRepository : GenericRepository<ListingImage>, IListingImageRepository
    {
        public ListingImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}