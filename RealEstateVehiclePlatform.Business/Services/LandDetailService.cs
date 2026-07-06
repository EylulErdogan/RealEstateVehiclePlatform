using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class LandDetailService : ILandDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LandDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(LandDetail landDetail)
        {
            var listing = _unitOfWork.Listings.GetById(landDetail.ListingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            var exists = _unitOfWork.LandDetails.GetByFilter(x =>
                x.ListingId == landDetail.ListingId && !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilan için arsa detayı zaten mevcut.");

            _unitOfWork.LandDetails.Insert(landDetail);
            _unitOfWork.Save();
        }

        public LandDetail GetByListingId(int listingId)
        {
            return _unitOfWork.LandDetails
                .GetByFilter(x => x.ListingId == listingId && !x.IsDeleted);
        }

        public void Update(LandDetail landDetail)
        {
            var value = _unitOfWork.LandDetails.GetById(landDetail.Id);

            if (value == null)
                throw new Exception("Arsa detayı bulunamadı.");

            value.SquareMeters = landDetail.SquareMeters;
            value.ZoningStatus = landDetail.ZoningStatus;
            value.BlockNo = landDetail.BlockNo;
            value.ParcelNo = landDetail.ParcelNo;
            value.SheetNo = landDetail.SheetNo;
            value.IsSuitableForCredit = landDetail.IsSuitableForCredit;

            _unitOfWork.LandDetails.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.LandDetails.GetById(id);

            if (value == null)
                throw new Exception("Arsa detayı bulunamadı.");

            _unitOfWork.LandDetails.Delete(value);
            _unitOfWork.Save();
        }
    }
}