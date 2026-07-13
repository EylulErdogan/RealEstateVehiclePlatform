namespace RealEstateVehiclePlatform.WebUI.ViewModels.District
{
    public class DistrictViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;
    }
}