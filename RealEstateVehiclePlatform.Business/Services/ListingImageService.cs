using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class ListingImageService : IListingImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListingImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(ListingImage listingImage)
        {
            var listing = _unitOfWork.Listings.GetById(listingImage.ListingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            _unitOfWork.ListingImages.Insert(listingImage);
            _unitOfWork.Save();
        }

        public List<ListingImage> GetByListingId(int listingId)
        {
            return _unitOfWork.ListingImages
                .GetAll()
                .Where(x => x.ListingId == listingId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }

        public void SetMainImage(int imageId)
        {
            var image = _unitOfWork.ListingImages.GetById(imageId);

            if (image == null)
                throw new Exception("Resim bulunamadı.");

            var listingImages = _unitOfWork.ListingImages
                .GetAll()
                .Where(x => x.ListingId == image.ListingId)
                .ToList();

            foreach (var item in listingImages)
            {
                item.IsMainImage = false;
                _unitOfWork.ListingImages.Update(item);
            }

            image.IsMainImage = true;
            _unitOfWork.ListingImages.Update(image);

            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var image = _unitOfWork.ListingImages.GetById(id);

            if (image == null)
                throw new Exception("Resim bulunamadı.");

            _unitOfWork.ListingImages.Delete(image);
            _unitOfWork.Save();
        }
    }
}