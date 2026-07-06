using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context)
        {
        }
    }
}