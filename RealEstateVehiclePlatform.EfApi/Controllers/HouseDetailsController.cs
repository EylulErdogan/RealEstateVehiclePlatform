using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseDetailsController : ControllerBase
    {
        private readonly IHouseDetailService _houseDetailService;

        public HouseDetailsController(
            IHouseDetailService houseDetailService)
        {
            _houseDetailService = houseDetailService;
        }

        [HttpPost("Create")]
        [Authorize]
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

        [HttpGet("GetByListing/{listingId:int}")]
        [AllowAnonymous]
        public IActionResult GetByListingId(int listingId)
        {
            var value = _houseDetailService
                .GetByListingId(listingId);

            if (value == null)
            {
                return NotFound("Ev detayı bulunamadı.");
            }

            return Ok(value);
        }

        [HttpPut("Update")]
        [Authorize]
        public IActionResult Update(HouseDetail houseDetail)
        {
            try
            {
                _houseDetailService.Update(houseDetail);

                return Ok("Ev detayı güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{id:int}")]
        [Authorize]
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