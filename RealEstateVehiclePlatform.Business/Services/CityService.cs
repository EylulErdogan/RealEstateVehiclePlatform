using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<City> GetAll()
        {
            return _unitOfWork.Cities.GetAll();
        }

        public City GetById(int id)
        {
            return _unitOfWork.Cities.GetById(id);
        }

        public void Create(City city)
        {
            var exists = _unitOfWork.Cities.GetByFilter(x => x.Name == city.Name && !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu şehir zaten mevcut.");

            _unitOfWork.Cities.Insert(city);
            _unitOfWork.Save();
        }

        public void Update(City city)
        {
            var value = _unitOfWork.Cities.GetById(city.Id);

            if (value == null)
                throw new Exception("Şehir bulunamadı.");

            value.Name = city.Name;

            _unitOfWork.Cities.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.Cities.GetById(id);

            if (value == null)
                throw new Exception("Şehir bulunamadı.");

            _unitOfWork.Cities.Delete(value);
            _unitOfWork.Save();
        }
    }
}