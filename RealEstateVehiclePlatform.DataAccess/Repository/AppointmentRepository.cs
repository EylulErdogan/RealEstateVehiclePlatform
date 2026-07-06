using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}