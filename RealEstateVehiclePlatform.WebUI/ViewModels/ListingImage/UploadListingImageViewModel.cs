using System.ComponentModel.DataAnnotations;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage
{
    public class UploadListingImageViewModel
    {
        public int ListingId { get; set; }

        [Required(ErrorMessage = "Lütfen bir görsel seçiniz.")]
        public IFormFile? ImageFile { get; set; }

        public bool IsMainImage { get; set; }

        public int DisplayOrder { get; set; } = 1;
    }
}