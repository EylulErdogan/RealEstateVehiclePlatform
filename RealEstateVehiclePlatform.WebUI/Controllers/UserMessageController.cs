using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Conversation;
using RealEstateVehiclePlatform.WebUI.ViewModels.Message;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class UserMessageController : Controller
    {
        private readonly ApiService _apiService;

        public UserMessageController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var conversations = await _apiService
                .GetListAsync<ConversationViewModel>(
                    "Conversations/MyConversations");

            return View(conversations);
        }

        [HttpGet]
        public async Task<IActionResult> Start(
            int listingId,
            int receiverId)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var conversation = await _apiService
                .PostAndGetAsync<object, ConversationViewModel>(
                    $"Conversations/Start/{listingId}/{receiverId}",
                    new { });

            if (conversation == null)
            {
                TempData["Error"] =
                    "Konuşma başlatılırken hata oluştu.";

                return RedirectToAction(
                    "Detail",
                    "Listing",
                    new { id = listingId });
            }

            return RedirectToAction(
                nameof(Detail),
                new { id = conversation.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var conversation = await _apiService
                .GetAsync<ConversationViewModel>(
                    $"Conversations/{id}");

            if (conversation == null)
            {
                TempData["Error"] = "Konuşma bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var messages = await _apiService
                .GetListAsync<MessageViewModel>(
                    $"Messages/ConversationMessages/{id}");

            var currentUserId =
                HttpContext.Session.GetInt32("UserId") ?? 0;

            var receiverId =
                conversation.UserOneId == currentUserId
                    ? conversation.UserTwoId
                    : conversation.UserOneId;

            var model = new ConversationDetailViewModel
            {
                ConversationId = conversation.Id,
                ListingId = conversation.ListingId,
                CurrentUserId = currentUserId,
                ReceiverId = receiverId,
                Messages = messages,
                NewMessage = new SendMessageViewModel
                {
                    ConversationId = conversation.Id,
                    ReceiverId = receiverId
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Send(
            SendMessageViewModel model)
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(model.Content))
            {
                TempData["Error"] = "Mesaj içeriği boş olamaz.";

                return RedirectToAction(
                    nameof(Detail),
                    new { id = model.ConversationId });
            }

            var result = await _apiService.PostAsync(
                "Messages/Send",
                model);

            if (!result)
            {
                TempData["Error"] =
                    "Mesaj gönderilirken hata oluştu.";
            }

            return RedirectToAction(
                nameof(Detail),
                new { id = model.ConversationId });
        }
    }
}