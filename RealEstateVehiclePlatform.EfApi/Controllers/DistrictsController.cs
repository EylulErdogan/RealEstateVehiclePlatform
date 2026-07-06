using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictsController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var values = _districtService.GetAll();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var value = _districtService.GetById(id);

            if (value == null)
                return NotFound("İlçe bulunamadı.");

            return Ok(value);
        }

        [HttpGet("GetByCity/{cityId}")]
        public IActionResult GetByCityId(int cityId)
        {
            var values = _districtService.GetByCityId(cityId);
            return Ok(values);
        }

        [HttpPost]
        public IActionResult Create(District district)
        {
            try
            {
                _districtService.Create(district);
                return Ok("İlçe başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(District district)
        {
            try
            {
                _districtService.Update(district);
                return Ok("İlçe başarıyla güncellendi.");
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
                _districtService.Delete(id);
                return Ok("İlçe başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}