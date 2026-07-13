using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IListingImageService
    {
        void Create(ListingImage listingImage);

        ListingImage? GetById(int id);

        List<ListingImage> GetByListingId(int listingId);

        void SetMainImage(int imageId);

        void Delete(int id);
    }
}