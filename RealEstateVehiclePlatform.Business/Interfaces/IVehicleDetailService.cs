using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IVehicleDetailService
    {
        void Create(VehicleDetail vehicleDetail);

        VehicleDetail GetByListingId(int listingId);

        void Update(VehicleDetail vehicleDetail);

        void Delete(int id);
    }
}