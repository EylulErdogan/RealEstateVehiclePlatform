using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;

        public MessagesController(
            IMessageService messageService,
            IConversationService conversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
        }

        [HttpPost("Send")]
        public IActionResult SendMessage(Message message)
        {
            try
            {
                var userIdValue = User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var senderId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                var conversation =
                    _conversationService.GetById(
                        message.ConversationId);

                if (conversation == null)
                    return NotFound("Konuşma bulunamadı.");

                if (conversation.UserOneId != senderId &&
                    conversation.UserTwoId != senderId)
                {
                    return Forbid();
                }

                var receiverId =
                    conversation.UserOneId == senderId
                        ? conversation.UserTwoId
                        : conversation.UserOneId;

                message.SenderId = senderId;
                message.ReceiverId = receiverId;

                _messageService.SendMessage(message);

                return Ok("Mesaj gönderildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConversationMessages/{conversationId}")]
        public IActionResult GetConversationMessages(
            int conversationId)
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var conversation =
                _conversationService.GetById(conversationId);

            if (conversation == null)
                return NotFound("Konuşma bulunamadı.");

            if (conversation.UserOneId != userId &&
                conversation.UserTwoId != userId)
            {
                return Forbid();
            }

            var values =
                _messageService.GetConversationMessages(
                    conversationId);

            return Ok(values);
        }

        [HttpPut("MarkAsRead/{messageId}")]
        public IActionResult MarkAsRead(int messageId)
        {
            try
            {
                _messageService.MarkAsRead(messageId);

                return Ok("Mesaj okundu olarak işaretlendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}