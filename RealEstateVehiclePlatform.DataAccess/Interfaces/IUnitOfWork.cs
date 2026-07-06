namespace RealEstateVehiclePlatform.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IListingRepository Listings { get; }
        ICityRepository Cities { get; }
        IDistrictRepository Districts { get; }
        IListingTypeRepository ListingTypes { get; }
        IFavoriteRepository Favorites { get; }
        IAppointmentRepository Appointments { get; }
        IMessageRepository Messages { get; }
        IHouseDetailRepository HouseDetails { get; }
        ILandDetailRepository LandDetails { get; }
        IVehicleDetailRepository VehicleDetails { get; }
        IListingImageRepository ListingImages { get; }
        IConversationRepository Conversations { get; }

        int Save();
    }
}