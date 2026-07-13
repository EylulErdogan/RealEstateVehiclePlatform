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

        [Range(1, double.MaxValue,
            ErrorMessage = "Fiyat sıfırdan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Kategori seçiniz.")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "İlan tipi seçiniz.")]
        public int ListingTypeId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Şehir seçiniz.")]
        public int CityId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "İlçe seçiniz.")]
        public int DistrictId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public List<SelectListItem> ListingTypes { get; set; } = new();

        public List<SelectListItem> Cities { get; set; } = new();

        public List<SelectListItem> Districts { get; set; } = new();
    }
}