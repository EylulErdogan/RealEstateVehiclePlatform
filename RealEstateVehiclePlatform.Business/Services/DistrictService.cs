using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DistrictService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<District> GetAll()
        {
            return _unitOfWork.Districts.GetAll();
        }

        public District GetById(int id)
        {
            return _unitOfWork.Districts.GetById(id);
        }

        public List<District> GetByCityId(int cityId)
        {
            return _unitOfWork.Districts
                .GetAll()
                .Where(x => x.CityId == cityId)
                .ToList();
        }

        public void Create(District district)
        {
            var city = _unitOfWork.Cities.GetById(district.CityId);

            if (city == null)
                throw new Exception("Şehir bulunamadı.");

            var exists = _unitOfWork.Districts.GetByFilter(x =>
                x.Name == district.Name &&
                x.CityId == district.CityId &&
                !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilçe zaten mevcut.");

            _unitOfWork.Districts.Insert(district);
            _unitOfWork.Save();
        }

        public void Update(District district)
        {
            var value = _unitOfWork.Districts.GetById(district.Id);

            if (value == null)
                throw new Exception("İlçe bulunamadı.");

            value.Name = district.Name;
            value.CityId = district.CityId;

            _unitOfWork.Districts.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.Districts.GetById(id);

            if (value == null)
                throw new Exception("İlçe bulunamadı.");

            _unitOfWork.Districts.Delete(value);
            _unitOfWork.Save();
        }
    }
}