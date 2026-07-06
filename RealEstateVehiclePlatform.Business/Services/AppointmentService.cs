using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.Enums;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(Appointment appointment)
        {
            var listing = _unitOfWork.Listings.GetById(appointment.ListingId);

            if (listing == null)
                throw new Exception("İlan bulunamadı.");

            var exists = _unitOfWork.Appointments.GetByFilter(x =>
                x.ListingId == appointment.ListingId &&
                x.AppointmentDate == appointment.AppointmentDate &&
                !x.IsDeleted);

            if (exists != null)
                throw new Exception("Bu ilan için seçilen saatte zaten randevu var.");

            appointment.Status = AppointmentStatus.Pending;

            _unitOfWork.Appointments.Insert(appointment);
            _unitOfWork.Save();
        }

        public List<Appointment> GetUserAppointments(int userId)
        {
            return _unitOfWork.Appointments
                .GetAll()
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public List<Appointment> GetListingAppointments(int listingId)
        {
            return _unitOfWork.Appointments
                .GetAll()
                .Where(x => x.ListingId == listingId)
                .ToList();
        }

        public void Approve(int id)
        {
            ChangeStatus(id, AppointmentStatus.Approved);
        }

        public void Reject(int id)
        {
            ChangeStatus(id, AppointmentStatus.Rejected);
        }

        public void Complete(int id)
        {
            ChangeStatus(id, AppointmentStatus.Completed);
        }

        public void Cancel(int id)
        {
            ChangeStatus(id, AppointmentStatus.Cancelled);
        }

        private void ChangeStatus(int id, AppointmentStatus status)
        {
            var appointment = _unitOfWork.Appointments.GetById(id);

            if (appointment == null)
                throw new Exception("Randevu bulunamadı.");

            appointment.Status = status;

            _unitOfWork.Appointments.Update(appointment);
            _unitOfWork.Save();
        }
    }
}