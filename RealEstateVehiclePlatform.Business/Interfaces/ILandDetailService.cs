using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface ILandDetailService
    {
        void Create(LandDetail landDetail);
        LandDetail GetByListingId(int listingId);
        void Update(LandDetail landDetail);
        void Delete(int id);
    }
}