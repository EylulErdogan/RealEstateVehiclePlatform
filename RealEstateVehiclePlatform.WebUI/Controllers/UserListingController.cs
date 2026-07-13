using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.District;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingType;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserListingController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ListingImageHelperService _listingImageHelperService;

        public UserListingController(
            ApiService apiService,
            ListingImageHelperService listingImageHelperService)
        {
            _apiService = apiService;
            _listingImageHelperService = listingImageHelperService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var values = await _apiService
                .GetListAsync<ListingViewModel>("Listings/MyListings");

            await _listingImageHelperService.FillMainImagesAsync(values);

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CreateListingWizardViewModel
            {
                ListingNo = $"ILN-{DateTime.Now:yyyyMMddHHmmss}"
            };

            await FillDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateListingWizardViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Title))
                missingFields.Add("İlan başlığı");

            if (string.IsNullOrWhiteSpace(model.Description))
                missingFields.Add("Açıklama");

            if (string.IsNullOrWhiteSpace(model.Address))
                missingFields.Add("Adres");

            if (model.Price <= 0)
                missingFields.Add("Fiyat");

            if (model.CategoryId <= 0)
                missingFields.Add("Kategori");

            if (model.ListingTypeId <= 0)
                missingFields.Add("İlan tipi");

            if (model.CityId <= 0)
                missingFields.Add("Şehir");

            if (model.DistrictId <= 0)
                missingFields.Add("İlçe");

            if (missingFields.Any())
            {
                await FillDropdowns(model);

                TempData["Error"] =
                    $"Şu alanları kontrol ediniz: {string.Join(", ", missingFields)}.";

                return View(model);
            }
            {
                await FillDropdowns(model);

                TempData["Error"] =
                    "Lütfen zorunlu ilan bilgilerini eksiksiz doldurunuz.";

                return View(model);
            }

            var listingId = await _apiService
                .PostAndGetAsync<CreateListingWizardViewModel, int>(
                    "Listings",
                    model);

            if (listingId <= 0)
            {
                await FillDropdowns(model);

                TempData["Error"] =
                    "İlan oluşturulurken bir hata meydana geldi.";

                return View(model);
            }

            var categories = await _apiService
                .GetListAsync<CategoryViewModel>("Categories");

            var selectedCategory = categories
                .FirstOrDefault(x => x.Id == model.CategoryId)
                ?.Name;

            var detailResult = await CreateCategoryDetail(
                selectedCategory,
                listingId,
                model);

            if (!detailResult)
            {
                TempData["Warning"] =
                    "İlan oluşturuldu ancak kategori detayları kaydedilemedi.";
            }
            else
            {
                TempData["Success"] =
                    "İlanınız oluşturuldu ve yönetici onayına gönderildi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task FillDropdowns(
            CreateListingWizardViewModel model)
        {
            model.Categories = await GetCategorySelectList();
            model.ListingTypes = await GetListingTypeSelectList();
            model.Cities = await GetCitySelectList();
            model.Districts = await GetDistrictSelectList();
        }

        private async Task<List<SelectListItem>> GetCategorySelectList()
        {
            var values = await _apiService
                .GetListAsync<CategoryViewModel>("Categories");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetListingTypeSelectList()
        {
            var values = await _apiService
                .GetListAsync<ListingTypeViewModel>("ListingTypes");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetCitySelectList()
        {
            var values = await _apiService
                .GetListAsync<CityViewModel>("Cities");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetDistrictSelectList()
        {
            var values = await _apiService
                .GetListAsync<DistrictViewModel>("Districts");

            return values.Select(x => new SelectListItem
            {
                Text = $"{x.Name} / {x.CityName}",
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task<bool> CreateCategoryDetail(
            string? categoryName,
            int listingId,
            CreateListingWizardViewModel model)
        {
            if (string.Equals(
                    categoryName,
                    "Ev",
                    StringComparison.OrdinalIgnoreCase))
            {
                return await _apiService.PostAsync(
                    "HouseDetails/Create",
                    new
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

            if (string.Equals(
                    categoryName,
                    "Arsa",
                    StringComparison.OrdinalIgnoreCase))
            {
                return await _apiService.PostAsync(
                    "LandDetails/Create",
                    new
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

            if (string.Equals(
                    categoryName,
                    "Araç",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    categoryName,
                    "Arac",
                    StringComparison.OrdinalIgnoreCase))
            {
                return await _apiService.PostAsync(
                    "VehicleDetails/Create",
                    new
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

            return true;
        }
    }
}