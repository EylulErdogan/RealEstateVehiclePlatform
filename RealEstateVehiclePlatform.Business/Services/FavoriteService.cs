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

        public bool ToggleFavorite(int userId, int listingId)
        {
            var favorite = _unitOfWork.Favorites
                .GetByUserAndListing(userId, listingId);

            if (favorite == null)
            {
                _unitOfWork.Favorites.Insert(new Favorite
                {
                    UserId = userId,
                    ListingId = listingId
                });

                _unitOfWork.Save();

                return true;
            }

            _unitOfWork.Favorites.Delete(favorite);

            _unitOfWork.Save();

            return false;
        }

        public bool IsFavorite(int userId, int listingId)
        {
            return _unitOfWork.Favorites
                .GetByUserAndListing(userId, listingId) != null;
        }

        public List<Favorite> GetUserFavorites(int userId)
        {
            return _unitOfWork.Favorites
                .GetAll()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToList();
        }
    }
}