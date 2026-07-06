using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class VehicleDetailRepository : GenericRepository<VehicleDetail>, IVehicleDetailRepository
    {
        public VehicleDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}