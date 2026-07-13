namespace RealEstateVehiclePlatform.WebUI.ViewModels.Appointment
{
    public class UserAppointmentViewModel
    {
        public int Id { get; set; }

        public int ListingId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; } = string.Empty;

        public int Status { get; set; }
    }
}