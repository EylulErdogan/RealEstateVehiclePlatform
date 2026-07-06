using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface ICityService
    {
        List<City> GetAll();
        City GetById(int id);
        void Create(City city);
        void Update(City city);
        void Delete(int id);
    }
}