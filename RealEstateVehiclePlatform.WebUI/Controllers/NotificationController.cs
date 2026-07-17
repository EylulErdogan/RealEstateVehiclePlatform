using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Appointment;
using RealEstateVehiclePlatform.WebUI.ViewModels.Conversation;
using RealEstateVehiclePlatform.WebUI.ViewModels.Message;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApiService _apiService;

        public NotificationController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var token = HttpContext.Session.GetString("Token");
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(token) || !currentUserId.HasValue)
            {
                return Json(new { success = false, message = "Oturum açık değil." });
            }

            var notifications = new List<object>();
            int unreadCount = 0;

            try
            {
                // 1. Okunmamış Sohbet Mesajlarını Çek
                var conversations = await _apiService.GetListAsync<ConversationViewModel>("Conversations/MyConversations");
                if (conversations != null)
                {
                    foreach (var conv in conversations)
                    {
                        var messages = await _apiService.GetListAsync<MessageViewModel>($"Messages/ConversationMessages/{conv.Id}");
                        if (messages != null)
                        {
                            var unreadMsgs = messages.Where(m => !m.IsRead && m.ReceiverId == currentUserId.Value).ToList();
                            if (unreadMsgs.Any())
                            {
                                unreadCount += unreadMsgs.Count;
                                var lastMsg = unreadMsgs.OrderByDescending(m => m.CreatedDate).First();
                                notifications.Add(new
                                {
                                    title = "Yeni Mesaj",
                                    message = lastMsg.Content.Length > 35 ? lastMsg.Content.Substring(0, 32) + "..." : lastMsg.Content,
                                    url = $"/UserMessage/Detail/{conv.Id}",
                                    icon = "bi-chat-dots-fill text-primary",
                                    time = lastMsg.CreatedDate.ToString("HH:mm"),
                                    type = "Message"
                                });
                            }
                        }
                    }
                }

                // 2. Son 3 Günlük Randevuları ve Durum Değişikliklerini Çek
                var appointments = await _apiService.GetListAsync<UserAppointmentViewModel>("Appointments/MyAppointments");
                if (appointments != null)
                {
                    var recentAppointments = appointments
                        .Where(a => a.AppointmentDate >= DateTime.Now.AddDays(-3))
                        .OrderByDescending(a => a.AppointmentDate)
                        .ToList();

                    foreach (var app in recentAppointments)
                    {
                        string statusText = "";
                        string icon = "";
                        bool shouldNotify = false;

                        // MyAppointments sadece oturum açan kişiye ait randevuları döner. 
                        // Doğrudan durum kontrolü yapabiliriz.
                        if (app.Status == 2) // Approved
                        {
                            statusText = "Randevu talebiniz onaylandı.";
                            icon = "bi-calendar-check-fill text-success";
                            shouldNotify = true;
                        }
                        else if (app.Status == 3) // Rejected
                        {
                            statusText = "Randevu talebiniz reddedildi.";
                            icon = "bi-calendar-x-fill text-danger";
                            shouldNotify = true;
                        }

                        if (shouldNotify)
                        {
                            notifications.Add(new
                            {
                                title = "Randevu Güncellemesi",
                                message = statusText,
                                url = "/UserAppointment",
                                icon = icon,
                                time = app.AppointmentDate.ToString("dd.MM HH:mm"),
                                type = "Appointment"
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Hata durumunda sessizce boş dönebiliriz.
            }

            return Json(new { success = true, data = notifications, unreadCount = unreadCount });
        }
    }
}