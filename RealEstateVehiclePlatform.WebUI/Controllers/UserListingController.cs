using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateVehiclePlatform.Entities.DTOs.HouseDetailDtos;
using RealEstateVehiclePlatform.Entities.DTOs.LandDetailDtos;
using RealEstateVehiclePlatform.Entities.DTOs.VehicleDetailDtos;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.District;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;
using RealEstateVehiclePlatform.WebUI.ViewModels.ListingType;
using System.Text;
using System.Text.Json;

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
                ListingNo = $"ILN-{DateTime.Now:yyyyMMddHHmmss}",
                Districts = new List<SelectListItem>()
            };

            model.Categories = await GetCategorySelectList();
            model.ListingTypes = await GetListingTypeSelectList();
            model.Cities = await GetCitySelectList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateListingWizardViewModel viewModel)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    var parts = token.Split('.');
                    if (parts.Length > 1)
                    {
                        var payload = parts[1];
                        payload = payload.Replace('-', '+').Replace('_', '/');
                        switch (payload.Length % 4)
                        {
                            case 2: payload += "=="; break;
                            case 3: payload += "="; break;
                        }
                        var bytes = Convert.FromBase64String(payload);
                        var json = Encoding.UTF8.GetString(bytes);

                        using (var doc = JsonDocument.Parse(json))
                        {
                            if (doc.RootElement.TryGetProperty("nameid", out var idProp) ||
                                doc.RootElement.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out idProp))
                            {
                                if (int.TryParse(idProp.GetString(), out int uId))
                                {
                                    viewModel.UserId = uId;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Hata durumunda default kalır.
            }

            var missingFields = new List<string>();

            // --- 1. AŞAMA: ORTAK ALANLARIN DOĞRULANMASI ---
            if (string.IsNullOrWhiteSpace(viewModel.Title))
                missingFields.Add("İlan başlığı");

            if (string.IsNullOrWhiteSpace(viewModel.Description))
                missingFields.Add("Açıklama");

            if (string.IsNullOrWhiteSpace(viewModel.Address))
                missingFields.Add("Adres");

            if (viewModel.Price <= 0)
                missingFields.Add("Fiyat (Sıfırdan büyük olmalıdır)");

            if (viewModel.CategoryId <= 0)
                missingFields.Add("Kategori seçimi");

            if (viewModel.ListingTypeId <= 0)
                missingFields.Add("İlan tipi seçimi");

            if (viewModel.CityId <= 0)
                missingFields.Add("Şehir seçimi");

            if (viewModel.DistrictId <= 0)
                missingFields.Add("İlçe seçimi");

            // --- 2. AŞAMA: KATEGORİYE ÖZEL MANTIKSAL DETAY DOĞRULAMASI ---
            try
            {
                var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
                var selectedCategory = categories.FirstOrDefault(x => x.Id == viewModel.CategoryId)?.Name;

                if (string.Equals(selectedCategory, "Ev", StringComparison.OrdinalIgnoreCase))
                {
                    if (!viewModel.RoomCount.HasValue || viewModel.RoomCount <= 0) missingFields.Add("Oda Sayısı (Sıfırdan büyük olmalıdır)");
                    if (!viewModel.LivingRoomCount.HasValue || viewModel.LivingRoomCount < 0) missingFields.Add("Salon Sayısı");

                    if (!viewModel.GrossSquareMeters.HasValue || viewModel.GrossSquareMeters <= 0) missingFields.Add("Brüt Metrekare (Sıfırdan büyük olmalıdır)");
                    if (!viewModel.NetSquareMeters.HasValue || viewModel.NetSquareMeters <= 0) missingFields.Add("Net Metrekare (Sıfırdan büyük olmalıdır)");

                    // Mantıksal Emlak Kuralları
                    if (viewModel.NetSquareMeters.HasValue && viewModel.GrossSquareMeters.HasValue && viewModel.NetSquareMeters > viewModel.GrossSquareMeters)
                        missingFields.Add("Net Alan, Brüt Alandan büyük olamaz");

                    if (!viewModel.BuildingAge.HasValue || viewModel.BuildingAge < 0) missingFields.Add("Bina Yaşı");
                    if (!viewModel.FloorNumber.HasValue) missingFields.Add("Bulunduğu Kat");
                    if (!viewModel.TotalFloors.HasValue || viewModel.TotalFloors <= 0) missingFields.Add("Toplam Kat Sayısı");

                    if (viewModel.FloorNumber.HasValue && viewModel.TotalFloors.HasValue && viewModel.FloorNumber > viewModel.TotalFloors)
                        missingFields.Add("Bulunduğu Kat, Binadaki Toplam Kat Sayısından büyük olamaz");

                    if (string.IsNullOrWhiteSpace(viewModel.HeatingType)) missingFields.Add("Isıtma Tipi");
                }
                else if (string.Equals(selectedCategory, "Arsa", StringComparison.OrdinalIgnoreCase))
                {
                    if (!viewModel.SquareMeters.HasValue || viewModel.SquareMeters <= 0) missingFields.Add("Metrekare (Sıfırdan büyük olmalıdır)");
                    if (string.IsNullOrWhiteSpace(viewModel.ZoningStatus)) missingFields.Add("İmar Durumu");
                    if (string.IsNullOrWhiteSpace(viewModel.BlockNo)) missingFields.Add("Ada No");
                    if (string.IsNullOrWhiteSpace(viewModel.ParcelNo)) missingFields.Add("Parsel No");
                }
                else if (string.Equals(selectedCategory, "Araç", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(selectedCategory, "Arac", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(viewModel.Brand)) missingFields.Add("Araç Markası");
                    if (string.IsNullOrWhiteSpace(viewModel.Model)) missingFields.Add("Araç Modeli");

                    // Mantıksal Vasıta Kuralları
                    int maxYear = DateTime.Now.Year + 1;
                    if (!viewModel.Year.HasValue || viewModel.Year < 1900 || viewModel.Year > maxYear)
                        missingFields.Add($"Model Yılı (1900 - {maxYear} arasında olmalıdır)");

                    if (!viewModel.Kilometer.HasValue || viewModel.Kilometer < 0) missingFields.Add("Araç Kilometresi");
                    if (string.IsNullOrWhiteSpace(viewModel.FuelType)) missingFields.Add("Yakıt Tipi");
                    if (string.IsNullOrWhiteSpace(viewModel.GearType)) missingFields.Add("Vites Tipi");
                }
            }
            catch
            {
                // API bağlantı hatası durumunda ana akışın hata vermesini API controller'a bırakıyoruz.
            }

            if (missingFields.Any())
            {
                await FillDropdowns(viewModel);
                TempData["Error"] = $"Lütfen şu alanları kontrol ediniz: {string.Join(", ", missingFields)}.";
                return View(viewModel);
            }

            int listingId = 0;

            try
            {
                var apiModel = new
                {
                    viewModel.Title,
                    viewModel.ListingNo,
                    viewModel.Description,
                    viewModel.Price,
                    viewModel.Address,
                    viewModel.CategoryId,
                    viewModel.ListingTypeId,
                    viewModel.CityId,
                    viewModel.DistrictId,
                    viewModel.UserId
                };

                listingId = await _apiService
                    .PostAndGetAsync<object, int>(
                        "Listings",
                        apiModel);
            }
            catch (Exception ex)
            {
                await FillDropdowns(viewModel);
                TempData["Error"] = $"İlan kaydedilirken API hatası oluştu: {ex.Message}";
                return View(viewModel);
            }

            if (listingId <= 0)
            {
                await FillDropdowns(viewModel);
                TempData["Error"] = "API geçerli bir ilan ID değeri döndürmedi.";
                return View(viewModel);
            }

            try
            {
                var categories = await _apiService
                    .GetListAsync<CategoryViewModel>("Categories");

                var selectedCategory = categories
                    .FirstOrDefault(x => x.Id == viewModel.CategoryId)
                    ?.Name;

                var detailResult = await CreateCategoryDetail(
                    selectedCategory,
                    listingId,
                    viewModel);

                if (!detailResult)
                {
                    TempData["Warning"] = "İlan oluşturuldu ancak kategori detayları kaydedilemedi.";
                }
                else
                {
                    TempData["Success"] = "İlanınız oluşturuldu. Şimdi ilan görsellerini ekleyebilirsiniz.";
                }
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"İlan oluşturuldu ancak detay bilgileri kaydedilemedi: {ex.Message}";
            }

            return RedirectToAction(
                "Index",
                "UserListingImage",
                new { listingId });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var listing = await _apiService
                .GetAsync<UserUpdateListingViewModel>($"Listings/MyListing/{id}");

            if (listing == null)
            {
                TempData["Error"] = "İlan bulunamadı veya bu ilana erişim yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Dropdown listelerini doldur
            listing.Categories = await GetCategorySelectList();
            listing.ListingTypes = await GetListingTypeSelectList();
            listing.Cities = await GetCitySelectList();
            listing.Districts = await GetDistrictSelectListByCity(listing.CityId);

            // Kategori detaylarını yükle
            try
            {
                var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
                var categoryName = categories.FirstOrDefault(x => x.Id == listing.CategoryId)?.Name;

                if (string.Equals(categoryName, "Ev", StringComparison.OrdinalIgnoreCase))
                {
                    var detail = await _apiService.GetAsync<HouseDetailDto>($"HouseDetails/GetByListing/{id}");
                    if (detail != null)
                    {
                        listing.RoomCount = detail.RoomCount;
                        listing.LivingRoomCount = detail.LivingRoomCount;
                        listing.GrossSquareMeters = detail.GrossSquareMeters;
                        listing.NetSquareMeters = detail.NetSquareMeters;
                        listing.BuildingAge = detail.BuildingAge;
                        listing.FloorNumber = detail.FloorNumber;
                        listing.TotalFloors = detail.TotalFloors;
                        listing.HeatingType = detail.HeatingType;
                        listing.HasBalcony = detail.HasBalcony;
                        listing.IsFurnished = detail.IsFurnished;
                    }
                }
                else if (string.Equals(categoryName, "Arsa", StringComparison.OrdinalIgnoreCase))
                {
                    var detail = await _apiService.GetAsync<LandDetailDto>($"LandDetails/GetByListing/{id}");
                    if (detail != null)
                    {
                        listing.SquareMeters = detail.SquareMeters;
                        listing.ZoningStatus = detail.ZoningStatus;
                        listing.BlockNo = detail.BlockNo;
                        listing.ParcelNo = detail.ParcelNo;
                        listing.SheetNo = detail.SheetNo;
                        listing.IsSuitableForCredit = detail.IsSuitableForCredit;
                    }
                }
                else if (string.Equals(categoryName, "Araç", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(categoryName, "Arac", StringComparison.OrdinalIgnoreCase))
                {
                    var detail = await _apiService.GetAsync<VehicleDetailDto>($"VehicleDetails/GetByListing/{id}");
                    if (detail != null)
                    {
                        listing.Brand = detail.Brand;
                        listing.Model = detail.Model;
                        listing.Year = detail.Year;
                        listing.Kilometer = detail.Kilometer;
                        listing.FuelType = detail.FuelType;
                        listing.GearType = detail.GearType;
                        listing.BodyType = detail.BodyType;
                        listing.Color = detail.Color;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"İlan detayları yüklenirken hata oluştu: {ex.Message}";
            }

            return View(listing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserUpdateListingViewModel viewModel)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Temel MVC Model Doğrulamaları
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await GetCategorySelectList();
                viewModel.ListingTypes = await GetListingTypeSelectList();
                viewModel.Cities = await GetCitySelectList();
                if (viewModel.CityId > 0)
                {
                    viewModel.Districts = await GetDistrictSelectListByCity(viewModel.CityId);
                }
                return View(viewModel);
            }

            var missingDetailFields = new List<string>();

            // --- KATEGORİYE ÖZEL MANTIKSAL DETAY DOĞRULAMASI ---
            try
            {
                var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
                var selectedCategory = categories.FirstOrDefault(x => x.Id == viewModel.CategoryId)?.Name;

                if (string.Equals(selectedCategory, "Ev", StringComparison.OrdinalIgnoreCase))
                {
                    if (!viewModel.RoomCount.HasValue || viewModel.RoomCount <= 0) missingDetailFields.Add("Oda Sayısı (Sıfırdan büyük olmalıdır)");
                    if (!viewModel.LivingRoomCount.HasValue || viewModel.LivingRoomCount < 0) missingDetailFields.Add("Salon Sayısı");

                    if (!viewModel.GrossSquareMeters.HasValue || viewModel.GrossSquareMeters <= 0) missingDetailFields.Add("Brüt Metrekare (Sıfırdan büyük olmalıdır)");
                    if (!viewModel.NetSquareMeters.HasValue || viewModel.NetSquareMeters <= 0) missingDetailFields.Add("Net Metrekare (Sıfırdan büyük olmalıdır)");

                    // Mantıksal Emlak Kuralları
                    if (viewModel.NetSquareMeters.HasValue && viewModel.GrossSquareMeters.HasValue && viewModel.NetSquareMeters > viewModel.GrossSquareMeters)
                        missingDetailFields.Add("Net Alan, Brüt Alandan büyük olamaz");

                    if (!viewModel.BuildingAge.HasValue || viewModel.BuildingAge < 0) missingDetailFields.Add("Bina Yaşı");
                    if (!viewModel.FloorNumber.HasValue) missingDetailFields.Add("Bulunduğu Kat");
                    if (!viewModel.TotalFloors.HasValue || viewModel.TotalFloors <= 0) missingDetailFields.Add("Toplam Kat Sayısı");

                    if (viewModel.FloorNumber.HasValue && viewModel.TotalFloors.HasValue && viewModel.FloorNumber > viewModel.TotalFloors)
                        missingDetailFields.Add("Bulunduğu Kat, Binadaki Toplam Kat Sayısından büyük olamaz");

                    if (string.IsNullOrWhiteSpace(viewModel.HeatingType)) missingDetailFields.Add("Isıtma Tipi");
                }
                else if (string.Equals(selectedCategory, "Arsa", StringComparison.OrdinalIgnoreCase))
                {
                    if (!viewModel.SquareMeters.HasValue || viewModel.SquareMeters <= 0) missingDetailFields.Add("Metrekare (Sıfırdan büyük olmalıdır)");
                    if (string.IsNullOrWhiteSpace(viewModel.ZoningStatus)) missingDetailFields.Add("İmar Durumu");
                    if (string.IsNullOrWhiteSpace(viewModel.BlockNo)) missingDetailFields.Add("Ada No");
                    if (string.IsNullOrWhiteSpace(viewModel.ParcelNo)) missingDetailFields.Add("Parsel No");
                }
                else if (string.Equals(selectedCategory, "Araç", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(selectedCategory, "Arac", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(viewModel.Brand)) missingDetailFields.Add("Araç Markası");
                    if (string.IsNullOrWhiteSpace(viewModel.Model)) missingDetailFields.Add("Araç Modeli");

                    // Mantıksal Vasıta Kuralları
                    int maxYear = DateTime.Now.Year + 1;
                    if (!viewModel.Year.HasValue || viewModel.Year < 1900 || viewModel.Year > maxYear)
                        missingDetailFields.Add($"Model Yılı (1900 - {maxYear} arasında olmalıdır)");

                    if (!viewModel.Kilometer.HasValue || viewModel.Kilometer < 0) missingDetailFields.Add("Araç Kilometresi");
                    if (string.IsNullOrWhiteSpace(viewModel.FuelType)) missingDetailFields.Add("Yakıt Tipi");
                    if (string.IsNullOrWhiteSpace(viewModel.GearType)) missingDetailFields.Add("Vites Tipi");
                }
            }
            catch
            {
                // API hatası durumunda API katmanı hatayı dönecektir
            }

            if (missingDetailFields.Any())
            {
                viewModel.Categories = await GetCategorySelectList();
                viewModel.ListingTypes = await GetListingTypeSelectList();
                viewModel.Cities = await GetCitySelectList();
                if (viewModel.CityId > 0)
                {
                    viewModel.Districts = await GetDistrictSelectListByCity(viewModel.CityId);
                }

                TempData["Error"] = $"Lütfen seçilen kategoriye ait şu detayları kontrol ediniz: {string.Join(", ", missingDetailFields)}.";
                return View(viewModel);
            }

            // 1. Temel ilan bilgilerini güncelle
            var updateModel = new
            {
                viewModel.Title,
                viewModel.Description,
                viewModel.Price,
                viewModel.Address,
                viewModel.CategoryId,
                viewModel.ListingTypeId,
                viewModel.CityId,
                viewModel.DistrictId
            };

            var listingUpdateResult = await _apiService.PutAsync($"Listings/UpdateMyListing/{viewModel.Id}", updateModel);
            if (!listingUpdateResult)
            {
                viewModel.Categories = await GetCategorySelectList();
                viewModel.ListingTypes = await GetListingTypeSelectList();
                viewModel.Cities = await GetCitySelectList();
                if (viewModel.CityId > 0)
                {
                    viewModel.Districts = await GetDistrictSelectListByCity(viewModel.CityId);
                }

                TempData["Error"] = "İlan güncellenirken API hatası oluştu.";
                return View(viewModel);
            }

            // 2. Kategori detaylarını güncelle (Yoksa yeni oluştur)
            try
            {
                var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
                var categoryName = categories.FirstOrDefault(x => x.Id == viewModel.CategoryId)?.Name;

                if (string.Equals(categoryName, "Ev", StringComparison.OrdinalIgnoreCase))
                {
                    var existingDetail = await _apiService.GetAsync<HouseDetailDto>($"HouseDetails/GetByListing/{viewModel.Id}");
                    if (existingDetail != null)
                    {
                        await _apiService.PutAsync("HouseDetails/Update", new
                        {
                            existingDetail.Id,
                            ListingId = viewModel.Id,
                            RoomCount = viewModel.RoomCount ?? 0,
                            LivingRoomCount = viewModel.LivingRoomCount ?? 0,
                            GrossSquareMeters = viewModel.GrossSquareMeters ?? 0,
                            NetSquareMeters = viewModel.NetSquareMeters ?? 0,
                            BuildingAge = viewModel.BuildingAge ?? 0,
                            FloorNumber = viewModel.FloorNumber ?? 0,
                            TotalFloors = viewModel.TotalFloors ?? 0,
                            HeatingType = viewModel.HeatingType ?? string.Empty,
                            HasBalcony = viewModel.HasBalcony,
                            IsFurnished = viewModel.IsFurnished
                        });
                    }
                    else
                    {
                        await _apiService.PostAsync("HouseDetails/Create", new
                        {
                            ListingId = viewModel.Id,
                            viewModel.RoomCount,
                            viewModel.LivingRoomCount,
                            viewModel.GrossSquareMeters,
                            viewModel.NetSquareMeters,
                            viewModel.BuildingAge,
                            viewModel.FloorNumber,
                            viewModel.TotalFloors,
                            viewModel.HeatingType,
                            viewModel.HasBalcony,
                            viewModel.IsFurnished
                        });
                    }
                }
                else if (string.Equals(categoryName, "Arsa", StringComparison.OrdinalIgnoreCase))
                {
                    var existingDetail = await _apiService.GetAsync<LandDetailDto>($"LandDetails/GetByListing/{viewModel.Id}");
                    if (existingDetail != null)
                    {
                        await _apiService.PutAsync("LandDetails/Update", new
                        {
                            existingDetail.Id,
                            ListingId = viewModel.Id,
                            SquareMeters = viewModel.SquareMeters ?? 0,
                            ZoningStatus = viewModel.ZoningStatus ?? string.Empty,
                            BlockNo = viewModel.BlockNo ?? string.Empty,
                            ParcelNo = viewModel.ParcelNo ?? string.Empty,
                            SheetNo = viewModel.SheetNo ?? string.Empty,
                            IsSuitableForCredit = viewModel.IsSuitableForCredit
                        });
                    }
                    else
                    {
                        await _apiService.PostAsync("LandDetails/Create", new
                        {
                            ListingId = viewModel.Id,
                            viewModel.SquareMeters,
                            viewModel.ZoningStatus,
                            viewModel.BlockNo,
                            viewModel.ParcelNo,
                            viewModel.SheetNo,
                            viewModel.IsSuitableForCredit
                        });
                    }
                }
                else if (string.Equals(categoryName, "Araç", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(categoryName, "Arac", StringComparison.OrdinalIgnoreCase))
                {
                    var existingDetail = await _apiService.GetAsync<VehicleDetailDto>($"VehicleDetails/GetByListing/{viewModel.Id}");
                    if (existingDetail != null)
                    {
                        await _apiService.PutAsync("VehicleDetails/Update", new
                        {
                            existingDetail.Id,
                            ListingId = viewModel.Id,
                            Brand = viewModel.Brand ?? string.Empty,
                            Model = viewModel.Model ?? string.Empty,
                            Year = viewModel.Year ?? 0,
                            Kilometer = viewModel.Kilometer ?? 0,
                            FuelType = viewModel.FuelType ?? string.Empty,
                            GearType = viewModel.GearType ?? string.Empty,
                            BodyType = viewModel.BodyType ?? string.Empty,
                            Color = viewModel.Color ?? string.Empty
                        });
                    }
                    else
                    {
                        await _apiService.PostAsync("VehicleDetails/Create", new
                        {
                            ListingId = viewModel.Id,
                            viewModel.Brand,
                            viewModel.Model,
                            viewModel.Year,
                            viewModel.Kilometer,
                            viewModel.FuelType,
                            viewModel.GearType,
                            viewModel.BodyType,
                            viewModel.Color
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"İlan güncellendi ancak detay bilgileri güncellenemedi: {ex.Message}";
            }

            TempData["Success"] = "İlan başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetDistrictSelectListByCity(int cityId)
        {
            var values = await _apiService
                .GetListAsync<DistrictViewModel>(
                    $"Districts/GetByCity/{cityId}");

            return values.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        private async Task FillDropdowns(CreateListingWizardViewModel model)
        {
            model.Categories = await GetCategorySelectList();
            model.ListingTypes = await GetListingTypeSelectList();
            model.Cities = await GetCitySelectList();

            if (model.CityId > 0)
            {
                model.Districts =
                    await GetDistrictSelectListByCity(model.CityId);
            }
            else
            {
                model.Districts = new List<SelectListItem>();
            }
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

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            if (cityId <= 0)
            {
                return Json(new List<object>());
            }

            var districts = await _apiService
                .GetListAsync<DistrictViewModel>(
                    $"Districts/GetByCity/{cityId}");

            var result = districts
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name
                })
                .ToList();

            return Json(result);
        }
    }
}