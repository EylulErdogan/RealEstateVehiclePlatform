using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserListingImageController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserListingImageController(
            ApiService apiService,
            IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int listingId)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Account");

            var listing = await _apiService
                .GetAsync<UserUpdateListingViewModel>(
                    $"Listings/MyListing/{listingId}");

            if (listing == null)
            {
                TempData["Error"] =
                    "İlan bulunamadı veya bu ilana erişim yetkiniz yok.";

                return RedirectToAction("Index", "UserListing");
            }

            var images = await _apiService.GetListAsync<ListingImageViewModel>(
                    $"ListingImages/GetByListing/{listingId}");

            ViewBag.ListingId = listingId;
            ViewBag.ListingTitle = listing.Title;
            ViewBag.ListingNo = listing.ListingNo;

            return View(images);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAjax(int listingId, IFormFile file)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return Json(new { success = false, message = "Oturum açmanız gerekmektedir." });

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Lütfen geçerli bir dosya seçiniz." });

            // 1. Dosya Uzantısı Kontrolü
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Sadece JPG, JPEG, PNG veya WEBP formatları desteklenmektedir." });

            // 2. Güvenlik: Dosya İmzası (Magic Number) Doğrulaması
            if (!IsValidImageSignature(file))
                return Json(new { success = false, message = "Güvenlik Engeli: Dosya içeriği geçerli bir resim formatında değil." });

            // 3. Dosya Boyutu Kontrolü (Maksimum 5MB)
            const long maximumFileSize = 5 * 1024 * 1024;
            if (file.Length > maximumFileSize)
                return Json(new { success = false, message = "Dosya boyutu en fazla 5 MB olabilir." });

            // İlan yetki ve varlık kontrolü
            var listing = await _apiService.GetAsync<UserUpdateListingViewModel>($"Listings/MyListing/{listingId}");
            if (listing == null)
                return Json(new { success = false, message = "İlan bulunamadı veya işlem yetkiniz yok." });

            // Fiziksel yükleme dizini oluşturma
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "listings");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var physicalPath = Path.Combine(uploadFolder, fileName);

            try
            {
                // Dosyayı diske kaydet
                await using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/images/listings/{fileName}";

                // Mevcut görselleri alarak sıralama değerini belirle
                var existingImages = await _apiService.GetListAsync<ListingImageViewModel>($"ListingImages/GetByListing/{listingId}");
                var isFirstImage = existingImages.Count == 0;
                var displayOrder = existingImages.Count + 1;

                var result = await _apiService.PostAsync(
                    "ListingImages/CreateMyListingImage",
                    new
                    {
                        ListingId = listingId,
                        ImageUrl = imageUrl,
                        IsMainImage = isFirstImage,
                        DisplayOrder = displayOrder
                    });

                if (!result)
                {
                    // Hata durumunda yüklenen fiziksel dosyayı temizle
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    return Json(new { success = false, message = "Görsel veritabanı kaydı oluşturulamadı." });
                }

                return Json(new { success = true, imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(physicalPath))
                    System.IO.File.Delete(physicalPath);

                return Json(new { success = false, message = $"Sistem hatası: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMain(int imageId, int listingId)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Account");

            var result = await _apiService.PutAsync<object>(
                $"ListingImages/SetMainImage/{imageId}",
                new { });

            TempData[result ? "Success" : "Error"] =
                result
                    ? "Ana görsel başarıyla güncellendi."
                    : "Ana görsel güncellenemedi.";

            return RedirectToAction(nameof(Index), new { listingId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int imageId, int listingId, string? imageUrl)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Account");

            var result = await _apiService.DeleteAsync(
                $"ListingImages/DeleteMyListingImage/{imageId}");

            if (result)
            {
                DeletePhysicalFile(imageUrl);
                TempData["Success"] = "Görsel başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Görsel silinirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index), new { listingId });
        }

        private void DeletePhysicalFile(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var relativePath = imageUrl
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var physicalPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                relativePath);

            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);
        }

        // Güvenlik İçin Dosya İmzası (Magic Number) Kontrol Metodu
        private bool IsValidImageSignature(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                if (stream.Length < 4) return false;

                var buffer = new byte[4];
                stream.Read(buffer, 0, 4);

                // JPEG, PNG, WEBP Byte Başlıkları
                var jpeg = new byte[] { 0xFF, 0xD8, 0xFF };
                var png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
                var webp = new byte[] { 0x52, 0x49, 0x46, 0x46 }; // RIFF header

                if (buffer.Take(3).SequenceEqual(jpeg)) return true;
                if (buffer.SequenceEqual(png)) return true;
                if (buffer.SequenceEqual(webp)) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}