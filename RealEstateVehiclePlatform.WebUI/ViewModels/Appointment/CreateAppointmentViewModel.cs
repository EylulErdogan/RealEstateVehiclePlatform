namespace RealEstateVehiclePlatform.WebUI.ViewModels.Appointment
{
    public class CreateAppointmentViewModel
    {
        public int ListingId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}