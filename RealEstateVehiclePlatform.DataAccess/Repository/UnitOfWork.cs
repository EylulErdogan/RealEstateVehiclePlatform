using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Categories = new CategoryRepository(_context);
            Listings = new ListingRepository(_context);
            Cities = new CityRepository(_context);
            Districts = new DistrictRepository(_context);
            ListingTypes = new ListingTypeRepository(_context);
            Favorites = new FavoriteRepository(_context);
            Appointments = new AppointmentRepository(_context);
            Messages = new MessageRepository(_context);
            HouseDetails = new HouseDetailRepository(_context);
            LandDetails = new LandDetailRepository(_context);
            VehicleDetails = new VehicleDetailRepository(_context);
            ListingImages = new ListingImageRepository(_context);
            Conversations = new ConversationRepository(_context);


        }

        public ICategoryRepository Categories { get; }

        public IListingRepository Listings { get; }

        public ICityRepository Cities { get; }

        public IDistrictRepository Districts { get; }

        public IListingTypeRepository ListingTypes { get; }

        public IFavoriteRepository Favorites { get; }

        public IAppointmentRepository Appointments { get; }

        public IMessageRepository Messages { get; }
        public IHouseDetailRepository HouseDetails { get; }

        public ILandDetailRepository LandDetails { get; }

        public IVehicleDetailRepository VehicleDetails { get; }

        public IListingImageRepository ListingImages { get; }
        public IConversationRepository Conversations { get; }


        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}