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

        [HttpGet]
        public async Task<IActionResult> Upload(int listingId)
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

            ViewBag.ListingTitle = listing.Title;

            return View(new UploadListingImageViewModel
            {
                ListingId = listingId,
                DisplayOrder = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(
            UploadListingImageViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Account");

            if (model.ImageFile == null ||
                model.ImageFile.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.ImageFile),
                    "Lütfen bir görsel seçiniz.");

                return View(model);
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path
                .GetExtension(model.ImageFile.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    nameof(model.ImageFile),
                    "Sadece JPG, JPEG, PNG veya WEBP yükleyebilirsiniz.");

                return View(model);
            }

            const long maximumFileSize = 5 * 1024 * 1024;

            if (model.ImageFile.Length > maximumFileSize)
            {
                ModelState.AddModelError(
                    nameof(model.ImageFile),
                    "Görsel en fazla 5 MB olabilir.");

                return View(model);
            }

            var uploadFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "images",
                "listings");

            Directory.CreateDirectory(uploadFolder);

            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            var physicalPath = Path.Combine(
                uploadFolder,
                fileName);

            await using (var stream =
                         new FileStream(
                             physicalPath,
                             FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            var imageUrl = $"/images/listings/{fileName}";

            var result = await _apiService.PostAsync(
                "ListingImages/CreateMyListingImage",
                new
                {
                    model.ListingId,
                    ImageUrl = imageUrl,
                    model.IsMainImage,
                    model.DisplayOrder
                });

            if (!result)
            {
                if (System.IO.File.Exists(physicalPath))
                    System.IO.File.Delete(physicalPath);

                TempData["Error"] =
                    "Görsel veritabanına kaydedilemedi.";

                return View(model);
            }

            TempData["Success"] =
                "İlan görseli başarıyla yüklendi.";

            return RedirectToAction(
                nameof(Index),
                new { listingId = model.ListingId });
        }

        [HttpPost]
        public async Task<IActionResult> SetMain(
            int imageId,
            int listingId)
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

            return RedirectToAction(
                nameof(Index),
                new { listingId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(
            int imageId,
            int listingId,
            string? imageUrl)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Account");

            var result = await _apiService.DeleteAsync(
                $"ListingImages/DeleteMyListingImage/{imageId}");

            if (result)
            {
                DeletePhysicalFile(imageUrl);

                TempData["Success"] =
                    "Görsel başarıyla silindi.";
            }
            else
            {
                TempData["Error"] =
                    "Görsel silinirken hata oluştu.";
            }

            return RedirectToAction(
                nameof(Index),
                new { listingId });
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
    }
}