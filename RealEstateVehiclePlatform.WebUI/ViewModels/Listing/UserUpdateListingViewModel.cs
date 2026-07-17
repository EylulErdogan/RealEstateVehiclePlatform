using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Listing
{
    public class UserUpdateListingViewModel
    {
        public int Id { get; set; }

        public string ListingNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlan başlığı zorunludur.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Fiyat sıfırdan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Kategori seçiniz.")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "İlan tipi seçiniz.")]
        public int ListingTypeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Şehir seçiniz.")]
        public int CityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "İlçe seçiniz.")]
        public int DistrictId { get; set; }

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
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> ListingTypes { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
        public List<SelectListItem> Districts { get; set; } = new();
    }
}