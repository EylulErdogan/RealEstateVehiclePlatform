using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class LandDetailRepository : GenericRepository<LandDetail>, ILandDetailRepository
    {
        public LandDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}