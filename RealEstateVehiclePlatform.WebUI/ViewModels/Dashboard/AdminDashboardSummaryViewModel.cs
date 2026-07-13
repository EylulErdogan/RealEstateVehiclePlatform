namespace RealEstateVehiclePlatform.WebUI.ViewModels.Dashboard
{
    public class AdminDashboardSummaryViewModel
    {
        public int TotalListingCount { get; set; }
        public int ActiveListingCount { get; set; }
        public int PendingListingCount { get; set; }
        public int UserCount { get; set; }
        public int AppointmentCount { get; set; }
        public int PendingAppointmentCount { get; set; }
        public int CategoryCount { get; set; }
        public int CityCount { get; set; }
    }
}