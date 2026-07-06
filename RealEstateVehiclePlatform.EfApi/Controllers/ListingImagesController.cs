using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingImagesController : ControllerBase
    {
        private readonly IListingImageService _listingImageService;

        public ListingImagesController(IListingImageService listingImageService)
        {
            _listingImageService = listingImageService;
        }

        [HttpPost("Create")]
        public IActionResult Create(ListingImage listingImage)
        {
            try
            {
                _listingImageService.Create(listingImage);
                return Ok("İlan resmi eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByListing/{listingId}")]
        public IActionResult GetByListingId(int listingId)
        {
            var values = _listingImageService.GetByListingId(listingId);
            return Ok(values);
        }

        [HttpPut("SetMainImage/{imageId}")]
        public IActionResult SetMainImage(int imageId)
        {
            try
            {
                _listingImageService.SetMainImage(imageId);
                return Ok("Ana resim güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _listingImageService.Delete(id);
                return Ok("İlan resmi silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}