using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class ListingTypeService : IListingTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListingTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<ListingType> GetAll()
        {
            return _unitOfWork.ListingTypes.GetAll();
        }

        public ListingType GetById(int id)
        {
            return _unitOfWork.ListingTypes.GetById(id);
        }

        public void Create(ListingType listingType)
        {
            var existingType = _unitOfWork.ListingTypes
                .GetByFilter(x => x.Name == listingType.Name && !x.IsDeleted);

            if (existingType != null)
            {
                throw new Exception("Bu ilan tipi zaten mevcut.");
            }

            _unitOfWork.ListingTypes.Insert(listingType);
            _unitOfWork.Save();
        }

        public void Update(ListingType listingType)
        {
            var value = _unitOfWork.ListingTypes.GetById(listingType.Id);

            if (value == null)
            {
                throw new Exception("İlan tipi bulunamadı.");
            }

            value.Name = listingType.Name;

            _unitOfWork.ListingTypes.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.ListingTypes.GetById(id);

            if (value == null)
            {
                throw new Exception("İlan tipi bulunamadı.");
            }

            _unitOfWork.ListingTypes.Delete(value);
            _unitOfWork.Save();
        }
    }
}