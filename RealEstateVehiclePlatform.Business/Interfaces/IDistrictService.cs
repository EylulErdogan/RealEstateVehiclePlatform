using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IDistrictService
    {
        List<District> GetAll();
        District GetById(int id);
        List<District> GetByCityId(int cityId);
        void Create(District district);
        void Update(District district);
        void Delete(int id);
    }
}