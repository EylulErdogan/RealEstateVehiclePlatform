using System.ComponentModel.DataAnnotations;

namespace RealEstateVehiclePlatform.WebUI.ViewModels.Profile
{
    public class UpdateUserProfileViewModel
    {
        [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string UserName { get; set; } = string.Empty;
    }
}