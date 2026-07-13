using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.District
{
    public class UpdateDistrictViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public List<SelectListItem>? Cities { get; set; }
    }
}