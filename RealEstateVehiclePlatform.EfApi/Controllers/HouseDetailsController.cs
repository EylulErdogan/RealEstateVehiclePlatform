using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HouseDetailsController : ControllerBase
    {
        private readonly IHouseDetailService _houseDetailService;

        public HouseDetailsController(IHouseDetailService houseDetailService)
        {
            _houseDetailService = houseDetailService;
        }

        [HttpPost("Create")]
        public IActionResult Create(HouseDetail houseDetail)
        {
            try
            {
                _houseDetailService.Create(houseDetail);
                return Ok("Ev detayı eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByListing/{listingId}")]
        public IActionResult GetByListingId(int listingId)
        {
            var value = _houseDetailService.GetByListingId(listingId);
            return Ok(value);
        }

        [HttpPut("Update")]
        public IActionResult Update(HouseDetail houseDetail)
        {
            try
            {
                _houseDetailService.Update(houseDetail);
                return Ok("Ev detayı guncellendi.");
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
                _houseDetailService.Delete(id);
                return Ok("Ev detayı silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}