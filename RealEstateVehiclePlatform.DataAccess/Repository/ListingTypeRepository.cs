using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class ListingTypeRepository : GenericRepository<ListingType>, IListingTypeRepository
    {
        public ListingTypeRepository(AppDbContext context) : base(context)
        {
        }
    }
}