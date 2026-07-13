using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleDetailsController : ControllerBase
    {
        private readonly IVehicleDetailService _vehicleDetailService;

        public VehicleDetailsController(IVehicleDetailService vehicleDetailService)
        {
            _vehicleDetailService = vehicleDetailService;
        }

        [HttpPost("Create")]
        public IActionResult Create(VehicleDetail vehicleDetail)
        {
            try
            {
                _vehicleDetailService.Create(vehicleDetail);
                return Ok("Arac detayı eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByListing/{listingId}")]
        public IActionResult GetByListingId(int listingId)
        {
            var value = _vehicleDetailService.GetByListingId(listingId);
            return Ok(value);
        }

        [HttpPut("Update")]
        public IActionResult Update(VehicleDetail vehicleDetail)
        {
            try
            {
                _vehicleDetailService.Update(vehicleDetail);
                return Ok("Araç detayı guncellendi.");
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
                _vehicleDetailService.Delete(id);
                return Ok("Araç detayı silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}