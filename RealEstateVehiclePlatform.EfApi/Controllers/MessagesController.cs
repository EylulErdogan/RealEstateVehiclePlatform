using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("Send")]
        public IActionResult SendMessage(Message message)
        {
            try
            {
                _messageService.SendMessage(message);
                return Ok("Mesaj gönderildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConversationMessages/{conversationId}")]
        public IActionResult GetConversationMessages(int conversationId)
        {
            var values = _messageService.GetConversationMessages(conversationId);
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