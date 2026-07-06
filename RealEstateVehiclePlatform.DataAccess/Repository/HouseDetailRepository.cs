using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class HouseDetailRepository : GenericRepository<HouseDetail>, IHouseDetailRepository
    {
        public HouseDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}