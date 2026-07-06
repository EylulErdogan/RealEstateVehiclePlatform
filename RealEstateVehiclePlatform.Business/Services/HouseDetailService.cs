using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class HouseDetailService : IHouseDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HouseDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(HouseDetail houseDetail)
        {
            var listing = _unitOfWork.Listings.GetById(houseDetail.ListingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            var exists = _unitOfWork.HouseDetails.GetByFilter(x =>
                x.ListingId == houseDetail.ListingId && !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilan için ev detayı zaten mevcut.");

            _unitOfWork.HouseDetails.Insert(houseDetail);
            _unitOfWork.Save();
        }

        public HouseDetail GetByListingId(int listingId)
        {
            return _unitOfWork.HouseDetails
                .GetByFilter(x => x.ListingId == listingId && !x.IsDeleted);
        }

        public void Update(HouseDetail houseDetail)
        {
            var value = _unitOfWork.HouseDetails.GetById(houseDetail.Id);

            if (value == null)
                throw new Exception("Ev detayı bulunamadı.");

            value.RoomCount = houseDetail.RoomCount;
            value.LivingRoomCount = houseDetail.LivingRoomCount;
            value.GrossSquareMeters = houseDetail.GrossSquareMeters;
            value.NetSquareMeters = houseDetail.NetSquareMeters;
            value.BuildingAge = houseDetail.BuildingAge;
            value.FloorNumber = houseDetail.FloorNumber;
            value.TotalFloors = houseDetail.TotalFloors;
            value.HeatingType = houseDetail.HeatingType;
            value.HasBalcony = houseDetail.HasBalcony;
            value.IsFurnished = houseDetail.IsFurnished;

            _unitOfWork.HouseDetails.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.HouseDetails.GetById(id);

            if (value == null)
                throw new Exception("Ev detayı bulunamadı.");

            _unitOfWork.HouseDetails.Delete(value);
            _unitOfWork.Save();
        }
    }
}