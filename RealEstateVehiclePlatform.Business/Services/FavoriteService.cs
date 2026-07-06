using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddFavorite(int userId, int listingId)
        {
            var listing = _unitOfWork.Listings.GetById(listingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            var exists = _unitOfWork.Favorites.GetByFilter(x =>
                x.UserId == userId &&
                x.ListingId == listingId &&
                !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilan zaten favorilerde.");

            var favorite = new Favorite
            {
                UserId = userId,
                ListingId = listingId
            };

            _unitOfWork.Favorites.Insert(favorite);
            _unitOfWork.Save();
        }

        public void RemoveFavorite(int userId, int listingId)
        {
            var favorite = _unitOfWork.Favorites.GetByFilter(x =>
                x.UserId == userId &&
                x.ListingId == listingId &&
                !x.IsDeleted);

            if (favorite == null)
                throw new Exception("Favori bulunamadı.");

            _unitOfWork.Favorites.Delete(favorite);
            _unitOfWork.Save();
        }

        public List<Favorite> GetUserFavorites(int userId)
        {
            return _unitOfWork.Favorites
                .GetAll()
                .Where(x => x.UserId == userId)
                .ToList();
        }
    }
}