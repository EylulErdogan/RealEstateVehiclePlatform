using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class VehicleDetailService : IVehicleDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(VehicleDetail vehicleDetail)
        {
            var listing = _unitOfWork.Listings.GetById(vehicleDetail.ListingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            var exists = _unitOfWork.VehicleDetails.GetByFilter(x =>
                x.ListingId == vehicleDetail.ListingId &&
                !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilan için araç detayı zaten mevcut.");

            _unitOfWork.VehicleDetails.Insert(vehicleDetail);
            _unitOfWork.Save();
        }

        public VehicleDetail GetByListingId(int listingId)
        {
            return _unitOfWork.VehicleDetails.GetByFilter(x =>
                x.ListingId == listingId &&
                !x.IsDeleted);
        }

        public void Update(VehicleDetail vehicleDetail)
        {
            var value = _unitOfWork.VehicleDetails.GetById(vehicleDetail.Id);

            if (value == null)
                throw new Exception("Araç detayı bulunamadı.");

            value.Brand = vehicleDetail.Brand;
            value.Model = vehicleDetail.Model;
            value.Year = vehicleDetail.Year;
            value.Kilometer = vehicleDetail.Kilometer;
            value.FuelType = vehicleDetail.FuelType;
            value.GearType = vehicleDetail.GearType;
            value.BodyType = vehicleDetail.BodyType;
            value.Color = vehicleDetail.Color;

            _unitOfWork.VehicleDetails.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.VehicleDetails.GetById(id);

            if (value == null)
                throw new Exception("Araç detayı bulunamadı.");

            _unitOfWork.VehicleDetails.Delete(value);
            _unitOfWork.Save();
        }
    }
}