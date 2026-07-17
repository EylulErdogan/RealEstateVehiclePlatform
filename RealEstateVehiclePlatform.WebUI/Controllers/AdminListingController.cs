using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.District;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingDetail; // Namespace eklendi
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingImage;  // Namespace eklendi
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingType;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class AdminListingController : AdminBaseController
    {
        private readonly ApiService _apiService;

        public AdminListingController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _apiService
                .GetListAsync<ListingViewModel>("Listings/AdminAll");

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var listing = await _apiService.GetAsync<ListingDetailViewModel>($"Listings/{id}");

            if (listing == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var images = await _apiService.GetListAsync<ListingImageViewModel>($"ListingImages/GetByListing/{id}");
            listing.Images = images;

            // Kategoriye göre detay bilgilerini çek
            if (string.Equals(listing.CategoryName, "Ev", StringComparison.OrdinalIgnoreCase))
            {
                listing.HouseDetail = await _apiService.GetAsync<HouseDetailViewModel>($"HouseDetails/GetByListing/{id}");
            }
            else if (string.Equals(listing.CategoryName, "Arsa", StringComparison.OrdinalIgnoreCase))
            {
                listing.LandDetail = await _apiService.GetAsync<LandDetailViewModel>($"LandDetails/GetByListing/{id}");
            }
            else if (string.Equals(listing.CategoryName, "Araç", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(listing.CategoryName, "Arac", StringComparison.OrdinalIgnoreCase))
            {
                listing.VehicleDetail = await _apiService.GetAsync<VehicleDetailViewModel>($"VehicleDetails/GetByListing/{id}");
            }

            return View(listing);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _apiService.PutAsync<object>(
                $"Listings/Approve/{id}",
                new { });

            TempData[result ? "Success" : "Error"] =
                result
                    ? "İlan başarıyla onaylandı."
                    : "İlan onaylanırken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _apiService.PutAsync<object>(
                $"Listings/Reject/{id}",
                new { });

            TempData[result ? "Success" : "Error"] =
                result
                    ? "İlan reddedildi."
                    : "İlan reddedilirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MakePassive(int id)
        {
            var result = await _apiService.PutAsync<object>(
                $"Listings/MakePassive/{id}",
                new { });

            TempData[result ? "Success" : "Error"] =
                result
                    ? "İlan pasife alındı."
                    : "İlan pasife alınırken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateListingWizardViewModel
            {
                ListingNo = "ILN-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Categories = await GetCategorySelectList(),
                ListingTypes = await GetListingTypeSelectList(),
                Cities = await GetCitySelectList(),
                Districts = await GetDistrictSelectList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateListingWizardViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Oturum bilgisi bulunamadı.";
                return RedirectToAction("Login", "Account");
            }

            model.UserId = userId.Value;

            var listingId = await _apiService.PostAndGetAsync<CreateListingWizardViewModel, int>("Listings", model);

            if (listingId == 0)
            {
                model.Categories = await GetCategorySelectList();
                model.ListingTypes = await GetListingTypeSelectList();
                model.Cities = await GetCitySelectList();
                model.Districts = await GetDistrictSelectList();

                TempData["Error"] = "Ilan eklenirken hata oluştu.";
                return View(model);
            }

            var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
            var selectedCategory = categories.FirstOrDefault(x => x.Id == model.CategoryId)?.Name;

            if (selectedCategory == "Ev")
            {
                await _apiService.PostAsync("HouseDetails/Create", new
                {
                    ListingId = listingId,
                    model.RoomCount,
                    model.LivingRoomCount,
                    model.GrossSquareMeters,
                    model.NetSquareMeters,
                    model.BuildingAge,
                    model.FloorNumber,
                    model.TotalFloors,
                    model.HeatingType,
                    model.HasBalcony,
                    model.IsFurnished
                });
            }
            else if (selectedCategory == "Arsa")
            {
                await _apiService.PostAsync("LandDetails/Create", new
                {
                    ListingId = listingId,
                    model.SquareMeters,
                    model.ZoningStatus,
                    model.BlockNo,
                    model.ParcelNo,
                    model.SheetNo,
                    model.IsSuitableForCredit
                });
            }
            else if (selectedCategory == "Araç")
            {
                await _apiService.PostAsync("VehicleDetails/Create", new
                {
                    ListingId = listingId,
                    model.Brand,
                    model.Model,
                    model.Year,
                    model.Kilometer,
                    model.FuelType,
                    model.GearType,
                    model.BodyType,
                    model.Color
                });
            }

            TempData["Success"] = "Ilan basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var value = await _apiService.GetAsync<UpdateListingViewModel>($"Listings/{id}");

            if (value == null)
            {
                TempData["Error"] = "Ilan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            value.Categories = await GetCategorySelectList();
            value.ListingTypes = await GetListingTypeSelectList();
            value.Cities = await GetCitySelectList();
            value.Districts = await GetDistrictSelectList();

            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateListingViewModel model)
        {
            var result = await _apiService.PutAsync("Listings", model);

            if (!result)
            {
                model.Categories = await GetCategorySelectList();
                model.ListingTypes = await GetListingTypeSelectList();
                model.Cities = await GetCitySelectList();
                model.Districts = await GetDistrictSelectList();

                TempData["Error"] = "Ilan güncellenirken hata oluştu.";
                return View(model);
            }

            TempData["Success"] = "Ilan basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _apiService.DeleteAsync($"Listings/{id}");

            if (result)
                TempData["Success"] = "Ilan basariyla silindi.";
            else
                TempData["Error"] = "Ilan silinirken hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetCategorySelectList()
        {
            var values = await _apiService.GetListAsync<CategoryViewModel>("Categories");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetListingTypeSelectList()
        {
            var values = await _apiService.GetListAsync<ListingTypeViewModel>("ListingTypes");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetCitySelectList()
        {
            var values = await _apiService.GetListAsync<CityViewModel>("Cities");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetDistrictSelectList()
        {
            var values = await _apiService.GetListAsync<DistrictViewModel>("Districts");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }
    }
}