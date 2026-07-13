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

        public ListingImage? GetById(int id)
        {
            return _unitOfWork.ListingImages
                .GetAll()
                .FirstOrDefault(x =>
                    x.Id == id &&
                    !x.IsDeleted);
        }

        public List<ListingImage> GetByListingId(int listingId)
        {
            return _unitOfWork.ListingImages
                .GetAll()
                .Where(x =>
                    x.ListingId == listingId &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.IsMainImage)
                .ThenBy(x => x.DisplayOrder)
                .ToList();
        }

        public void SetMainImage(int imageId)
        {
            var image = GetById(imageId);

            if (image == null)
                throw new Exception("Resim bulunamadı.");

            var listingImages = GetByListingId(image.ListingId);

            foreach (var item in listingImages)
            {
                item.IsMainImage = item.Id == imageId;

                _unitOfWork.ListingImages.Update(item);
            }

            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var image = GetById(id);

            if (image == null)
                throw new Exception("Resim bulunamadı.");

            var listingId = image.ListingId;
            var wasMainImage = image.IsMainImage;

            _unitOfWork.ListingImages.Delete(image);
            _unitOfWork.Save();

            if (!wasMainImage)
                return;

            var nextImage = GetByListingId(listingId)
                .FirstOrDefault();

            if (nextImage == null)
                return;

            nextImage.IsMainImage = true;

            _unitOfWork.ListingImages.Update(nextImage);
            _unitOfWork.Save();
        }
    }
}