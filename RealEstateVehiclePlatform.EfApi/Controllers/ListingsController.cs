using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

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
        public IActionResult GetAll()
        {
            var values = _listingService.GetAll();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var value = _listingService.GetById(id);

            if (value == null)
                return NotFound("İlan bulunamadı.");

            return Ok(value);
        }

        [HttpPost]
        public IActionResult Create(Listing listing)
        {
            try
            {
                _listingService.Create(listing);
                return Ok("İlan başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(Listing listing)
        {
            try
            {
                _listingService.Update(listing);
                return Ok("İlan başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _listingService.Delete(id);
                return Ok("İlan başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}