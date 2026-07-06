using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingTypesController : ControllerBase
    {
        private readonly IListingTypeService _listingTypeService;

        public ListingTypesController(IListingTypeService listingTypeService)
        {
            _listingTypeService = listingTypeService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var values = _listingTypeService.GetAll();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var value = _listingTypeService.GetById(id);

            if (value == null)
            {
                return NotFound("İlan tipi bulunamadı.");
            }

            return Ok(value);
        }

        [HttpPost]
        public IActionResult Create(ListingType listingType)
        {
            try
            {
                _listingTypeService.Create(listingType);
                return Ok("İlan tipi başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Update(ListingType listingType)
        {
            try
            {
                _listingTypeService.Update(listingType);
                return Ok("İlan tipi başarıyla güncellendi.");
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
                _listingTypeService.Delete(id);
                return Ok("İlan tipi başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}