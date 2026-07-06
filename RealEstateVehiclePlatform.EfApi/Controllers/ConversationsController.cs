using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("Start/{listingId}/{userOneId}/{userTwoId}")]
        public IActionResult StartConversation(int listingId, int userOneId, int userTwoId)
        {
            try
            {
                var conversation = _conversationService.StartConversation(listingId, userOneId, userTwoId);
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UserConversations/{userId}")]
        public IActionResult GetUserConversations(int userId)
        {
            var values = _conversationService.GetUserConversations(userId);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var value = _conversationService.GetById(id);

            if (value == null)
                return NotFound("Konuşma bulunamadı.");

            return Ok(value);
        }
    }
}