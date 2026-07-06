using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IAppointmentService
    {
        void Create(Appointment appointment);

        List<Appointment> GetUserAppointments(int userId);

        List<Appointment> GetListingAppointments(int listingId);

        void Approve(int id);

        void Reject(int id);

        void Complete(int id);

        void Cancel(int id);
    }
}