using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.ListingDtos;
using RealEstateVehiclePlatform.Entities.Enums;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var values = _listingService
                .GetAll()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == ListingStatus.Active)
                .OrderByDescending(x => x.Id)
                .ToList();

            return Ok(values);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var listing = _listingService.GetByIdWithDetails(id);

            if (listing == null || listing.IsDeleted)
                return NotFound("İlan bulunamadı.");

            var currentUserId = 0;

            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            int.TryParse(userIdValue, out currentUserId);

            var isAdmin = User.IsInRole("Admin");
            var isOwner = currentUserId > 0 &&
                          listing.UserId == currentUserId;

            /*
             * Aktif olmayan ilanı yalnızca:
             * - ilan sahibi
             * - admin
             * görüntüleyebilir.
             */
            if (listing.Status != ListingStatus.Active &&
                !isOwner &&
                !isAdmin)
            {
                return NotFound("İlan bulunamadı.");
            }

            /*
             * İlan sahibi veya admin kendi kontrolü için açtığında
             * görüntülenme sayısını artırmıyoruz.
             */
            if (listing.Status == ListingStatus.Active &&
                !isOwner &&
                !isAdmin)
            {
                listing.ViewCount++;

                _listingService.Update(listing);
            }

            var model = new ListingDetailDto
            {
                Id = listing.Id,
                ListingNo = listing.ListingNo,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                Address = listing.Address,
                ViewCount = listing.ViewCount,
                Status = (int)listing.Status,

                CategoryId = listing.CategoryId,
                CategoryName = listing.Category?.Name ?? string.Empty,

                ListingTypeId = listing.ListingTypeId,
                ListingTypeName = listing.ListingType?.Name ?? string.Empty,

                CityId = listing.CityId,
                CityName = listing.City?.Name ?? string.Empty,

                DistrictId = listing.DistrictId,
                DistrictName = listing.District?.Name ?? string.Empty,

                UserId = listing.UserId,
                OwnerFullName = listing.User?.FullName ?? string.Empty
            };

            return Ok(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Listing listing)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                listing.UserId = userId;
                listing.ViewCount = 0;
                listing.Status = ListingStatus.Pending;

                var listingId = _listingService.Create(listing);

                return Ok(listingId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update(Listing listing)
        {
            try
            {
                _listingService.Update(listing);
                return Ok("Ilan basariyla guncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("MyListings")]
        [Authorize]
        public IActionResult GetMyListings()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var values = _listingService
                .GetAll()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ToList();

            return Ok(values);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                _listingService.Delete(id);
                return Ok("Ilan basariyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteMyListing/{id}")]
        [Authorize]
        public IActionResult DeleteMyListing(int id)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var listing = _listingService.GetById(id);

                if (listing == null)
                    return NotFound("İlan bulunamadı.");

                if (listing.UserId != userId)
                    return Forbid();

                _listingService.Delete(id);

                return Ok("İlan başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("MyListing/{id}")]
        [Authorize]
        public IActionResult GetMyListing(int id)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var listing = _listingService.GetById(id);

            if (listing == null || listing.IsDeleted)
                return NotFound("İlan bulunamadı.");

            if (listing.UserId != userId)
                return Forbid();

            return Ok(listing);
        }
        [HttpPut("UpdateMyListing/{id}")]
        [Authorize]
        public IActionResult UpdateMyListing(
    int id,
    UpdateMyListingDto model)
        {
            try
            {
                var userIdValue = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var listing = _listingService.GetById(id);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                if (listing.UserId != userId)
                    return Forbid();

                if (string.IsNullOrWhiteSpace(model.Title))
                    return BadRequest("İlan başlığı zorunludur.");

                if (string.IsNullOrWhiteSpace(model.Description))
                    return BadRequest("İlan açıklaması zorunludur.");

                if (model.Price <= 0)
                    return BadRequest("Fiyat sıfırdan büyük olmalıdır.");

                listing.Title = model.Title.Trim();
                listing.Description = model.Description.Trim();
                listing.Price = model.Price;
                listing.Address = model.Address.Trim();

                listing.CategoryId = model.CategoryId;
                listing.ListingTypeId = model.ListingTypeId;
                listing.CityId = model.CityId;
                listing.DistrictId = model.DistrictId;

                // Kullanıcı güncellemesinden sonra yeniden admin onayı gerekir.
                listing.Status = ListingStatus.Pending;

                _listingService.Update(listing);

                return Ok("İlan güncellendi ve yeniden onaya gönderildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("AdminAll")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllForAdmin()
        {
            var values = _listingService
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ToList();

            return Ok(values);
        }
        [HttpPut("Approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            try
            {
                var listing = _listingService.GetById(id);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                listing.Status = ListingStatus.Active;

                _listingService.Update(listing);

                return Ok("İlan başarıyla onaylandı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Reject/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            try
            {
                var listing = _listingService.GetById(id);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                listing.Status = ListingStatus.Rejected;

                _listingService.Update(listing);

                return Ok("İlan reddedildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("MakePassive/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult MakePassive(int id)
        {
            try
            {
                var listing = _listingService.GetById(id);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                listing.Status = ListingStatus.Passive;

                _listingService.Update(listing);

                return Ok("İlan pasife alındı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}