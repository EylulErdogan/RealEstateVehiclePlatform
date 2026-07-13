using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Listing
{
    public class CreateListingWizardViewModel
    {
        // Ortak ilan bilgileri
        public string Title { get; set; } = string.Empty;

        public string ListingNo { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Address { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int ListingTypeId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }

        public int UserId { get; set; }

        // Ev detayları
        public int? RoomCount { get; set; }

        public int? LivingRoomCount { get; set; }

        public int? GrossSquareMeters { get; set; }

        public int? NetSquareMeters { get; set; }

        public int? BuildingAge { get; set; }

        public int? FloorNumber { get; set; }

        public int? TotalFloors { get; set; }

        public string? HeatingType { get; set; }

        public bool HasBalcony { get; set; }

        public bool IsFurnished { get; set; }

        // Arsa detayları
        public int? SquareMeters { get; set; }

        public string? ZoningStatus { get; set; }

        public string? BlockNo { get; set; }

        public string? ParcelNo { get; set; }

        public string? SheetNo { get; set; }

        public bool IsSuitableForCredit { get; set; }

        // Araç detayları
        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int? Year { get; set; }

        public int? Kilometer { get; set; }

        public string? FuelType { get; set; }

        public string? GearType { get; set; }

        public string? BodyType { get; set; }

        public string? Color { get; set; }

        // Dropdownlar
        public List<SelectListItem>? Categories { get; set; }

        public List<SelectListItem>? ListingTypes { get; set; }

        public List<SelectListItem>? Cities { get; set; }

        public List<SelectListItem>? Districts { get; set; }
    }
}