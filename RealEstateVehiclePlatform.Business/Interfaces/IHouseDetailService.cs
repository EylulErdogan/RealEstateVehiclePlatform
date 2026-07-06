using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IHouseDetailService
    {
        void Create(HouseDetail houseDetail);
        HouseDetail GetByListingId(int listingId);
        void Update(HouseDetail houseDetail);
        void Delete(int id);
    }
}