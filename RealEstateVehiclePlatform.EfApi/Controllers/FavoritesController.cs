using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("Toggle/{listingId}")]
        public IActionResult ToggleFavorite(int listingId)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var isFavorite = _favoriteService.ToggleFavorite(userId, listingId);

                return Ok(new
                {
                    isFavorite,
                    message = isFavorite
                        ? "İlan favorilere eklendi."
                        : "İlan favorilerden çıkarıldı."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("IsFavorite/{listingId}")]
        public IActionResult IsFavorite(int listingId)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var isFavorite = _favoriteService.IsFavorite(userId, listingId);

            return Ok(new
            {
                isFavorite
            });
        }

        [HttpGet("MyFavorites")]
        public IActionResult GetMyFavorites()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var values = _favoriteService.GetUserFavorites(userId);

            return Ok(values);
        }
    }
}