using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandDetailsController : ControllerBase
    {
        private readonly ILandDetailService _landDetailService;

        public LandDetailsController(
            ILandDetailService landDetailService)
        {
            _landDetailService = landDetailService;
        }

        [HttpPost("Create")]
        [Authorize]
        public IActionResult Create(LandDetail landDetail)
        {
            try
            {
                _landDetailService.Create(landDetail);

                return Ok("Arsa detayı eklendi.");
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
            var value = _landDetailService
                .GetByListingId(listingId);

            if (value == null)
            {
                return NotFound("Arsa detayı bulunamadı.");
            }

            return Ok(value);
        }

        [HttpPut("Update")]
        [Authorize]
        public IActionResult Update(LandDetail landDetail)
        {
            try
            {
                _landDetailService.Update(landDetail);

                return Ok("Arsa detayı güncellendi.");
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
                _landDetailService.Delete(id);

                return Ok("Arsa detayı silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}