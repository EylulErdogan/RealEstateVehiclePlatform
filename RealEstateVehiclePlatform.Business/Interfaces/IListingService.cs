using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IListingService
    {
        List<Listing> GetAll();

        Listing GetById(int id);

        int Create(Listing listing);

        void Update(Listing listing);

        void Delete(int id);
        Listing? GetByIdWithDetails(int id);
    }
}