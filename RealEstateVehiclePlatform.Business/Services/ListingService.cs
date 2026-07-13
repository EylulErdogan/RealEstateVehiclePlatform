using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class ListingService : IListingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Listing> GetAll()
        {
            return _unitOfWork.Listings.GetAll();
        }

        public Listing GetById(int id)
        {
            return _unitOfWork.Listings.GetById(id);
        }

        public int Create(Listing listing)
        {
            _unitOfWork.Listings.Insert(listing);
            _unitOfWork.Save();

            return listing.Id;
        }

        public void Update(Listing listing)
        {
            var value = _unitOfWork.Listings.GetById(listing.Id);

            if (value == null)
                throw new Exception("İlan bulunamadı.");

            value.Title = listing.Title;
            value.Description = listing.Description;
            value.Price = listing.Price;
            value.CityId = listing.CityId;
            value.DistrictId = listing.DistrictId;
            value.CategoryId = listing.CategoryId;
            value.ListingTypeId = listing.ListingTypeId;

            _unitOfWork.Listings.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.Listings.GetById(id);

            if (value == null)
                throw new Exception("İlan bulunamadı.");

            _unitOfWork.Listings.Delete(value);
            _unitOfWork.Save();
        }
        public Listing? GetByIdWithDetails(int id)
        {
            return _unitOfWork.Listings.GetByIdWithDetails(id);
        }
    }
}