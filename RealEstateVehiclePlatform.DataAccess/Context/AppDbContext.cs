using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ListingType> ListingTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<HouseDetail> HouseDetails { get; set; }
        public DbSet<LandDetail> LandDetails { get; set; }
        public DbSet<VehicleDetail> VehicleDetails { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Listing>()
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedMessages)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversation>()
                .HasOne(x => x.UserOne)
                .WithMany(x => x.ConversationsAsUserOne)
                .HasForeignKey(x => x.UserOneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversation>()
                .HasOne(x => x.UserTwo)
                .WithMany(x => x.ConversationsAsUserTwo)
                .HasForeignKey(x => x.UserTwoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HouseDetail>()
                .HasOne(x => x.Listing)
                .WithOne(x => x.HouseDetail)
                .HasForeignKey<HouseDetail>(x => x.ListingId);

            builder.Entity<LandDetail>()
                .HasOne(x => x.Listing)
                .WithOne(x => x.LandDetail)
                .HasForeignKey<LandDetail>(x => x.ListingId);

            builder.Entity<VehicleDetail>()
                .HasOne(x => x.Listing)
                .WithOne(x => x.VehicleDetail)
                .HasForeignKey<VehicleDetail>(x => x.ListingId);
            builder.Entity<Listing>()
    .HasOne(x => x.Category)
    .WithMany(x => x.Listings)
    .HasForeignKey(x => x.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .HasOne(x => x.ListingType)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.ListingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .HasOne(x => x.City)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .HasOne(x => x.District)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Listing>()
                .HasOne(x => x.User)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<District>()
                .HasOne(x => x.City)
                .WithMany(x => x.Districts)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}