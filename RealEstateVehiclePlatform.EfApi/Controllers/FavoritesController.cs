using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("Add/{userId}/{listingId}")]
        public IActionResult AddFavorite(int userId, int listingId)
        {
            try
            {
                _favoriteService.AddFavorite(userId, listingId);
                return Ok("İlan favorilere eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Remove/{userId}/{listingId}")]
        public IActionResult RemoveFavorite(int userId, int listingId)
        {
            try
            {
                _favoriteService.RemoveFavorite(userId, listingId);
                return Ok("İlan favorilerden çıkarıldı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UserFavorites/{userId}")]
        public IActionResult GetUserFavorites(int userId)
        {
            var values = _favoriteService.GetUserFavorites(userId);
            return Ok(values);
        }
    }
}