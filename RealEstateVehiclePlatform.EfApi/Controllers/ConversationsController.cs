using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(
            IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("Start/{listingId}/{receiverId}")]
        public IActionResult StartConversation(
            int listingId,
            int receiverId)
        {
            try
            {
                var userIdValue = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var currentUserId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var conversation =
                    _conversationService.StartConversation(
                        listingId,
                        currentUserId,
                        receiverId);

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MyConversations")]
        public IActionResult GetMyConversations()
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var values =
                _conversationService.GetUserConversations(userId);

            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var value = _conversationService.GetById(id);

            if (value == null)
                return NotFound("Konuşma bulunamadı.");

            if (value.UserOneId != userId &&
                value.UserTwoId != userId)
            {
                return Forbid();
            }

            return Ok(value);
        }
    }
}