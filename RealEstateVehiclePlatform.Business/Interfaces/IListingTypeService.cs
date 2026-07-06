using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IListingTypeService
    {
        List<ListingType> GetAll();

        ListingType GetById(int id);

        void Create(ListingType listingType);

        void Update(ListingType listingType);

        void Delete(int id);
    }
}