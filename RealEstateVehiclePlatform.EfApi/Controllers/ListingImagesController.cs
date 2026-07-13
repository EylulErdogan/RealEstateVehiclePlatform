using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.ListingImageDtos;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingImagesController : ControllerBase
    {
        private readonly IListingImageService _listingImageService;
        private readonly IListingService _listingService;

        public ListingImagesController(
            IListingImageService listingImageService,
            IListingService listingService)
        {
            _listingImageService = listingImageService;
            _listingService = listingService;
        }

        [HttpGet("GetByListing/{listingId}")]
        public IActionResult GetByListing(int listingId)
        {
            var values = _listingImageService
                .GetByListingId(listingId);

            return Ok(values);
        }

        [HttpPost("CreateMyListingImage")]
        [Authorize]
        public IActionResult CreateMyListingImage(
            CreateListingImageDto model)
        {
            try
            {
                var userIdValue = User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var listing = _listingService.GetById(model.ListingId);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                if (listing.UserId != userId)
                    return Forbid();

                if (string.IsNullOrWhiteSpace(model.ImageUrl))
                    return BadRequest("Görsel adresi boş olamaz.");

                var existingImages = _listingImageService
                    .GetByListingId(model.ListingId);

                var isFirstImage = existingImages.Count == 0;

                var listingImage = new ListingImage
                {
                    ListingId = model.ListingId,
                    ImageUrl = model.ImageUrl,
                    IsMainImage = false,
                    DisplayOrder = model.DisplayOrder
                };

                _listingImageService.Create(listingImage);

                if (model.IsMainImage || isFirstImage)
                {
                    _listingImageService.SetMainImage(listingImage.Id);
                }

                return Ok(listingImage.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("SetMainImage/{imageId}")]
        [Authorize]
        public IActionResult SetMainImage(int imageId)
        {
            try
            {
                var userIdValue = User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var image = _listingImageService.GetById(imageId);

                if (image == null)
                    return NotFound("Görsel bulunamadı.");

                var listing = _listingService.GetById(image.ListingId);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                if (listing.UserId != userId)
                    return Forbid();

                _listingImageService.SetMainImage(imageId);

                return Ok("Ana görsel güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteMyListingImage/{imageId}")]
        [Authorize]
        public IActionResult DeleteMyListingImage(int imageId)
        {
            try
            {
                var userIdValue = User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var image = _listingImageService.GetById(imageId);

                if (image == null)
                    return NotFound("Görsel bulunamadı.");

                var listing = _listingService.GetById(image.ListingId);

                if (listing == null || listing.IsDeleted)
                    return NotFound("İlan bulunamadı.");

                if (listing.UserId != userId)
                    return Forbid();

                _listingImageService.Delete(imageId);

                return Ok("Görsel başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}