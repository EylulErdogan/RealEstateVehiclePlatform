using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.WebUI.Services;
using RealEstateVehiclePlatform.WebUI.ViewModels.Category;
using RealEstateVehiclePlatform.WebUI.ViewModels.City;
using RealEstateVehiclePlatform.WebUI.ViewModels.Listing;

namespace RealEstateVehiclePlatform.WebUI.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ApiService _apiService;

        public ChatbotController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public async Task<IActionResult> Ask(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(new { reply = "Merhaba! Size nasıl yardımcı olabilirim?" });
            }

            // Türkçe karakter uyumlu küçük harfe dönüştürme
            var msg = message.ToLower(new System.Globalization.CultureInfo("tr-TR")).Trim();

            // Tüm aktif ilanları API üzerinden çekelim
            var listings = await _apiService.GetListAsync<ListingViewModel>("Listings");

            // Sadece Aktif ilanları filtreleyelim (Status = 2)
            listings = listings.Where(x => x.Status == 2).ToList();

            var categories = await _apiService.GetListAsync<CategoryViewModel>("Categories");
            var cities = await _apiService.GetListAsync<CityViewModel>("Cities");

            // --- YAPAY ZEKA SORGULAMA VE ANALİZ MOTORU ---

            // 1. Selamlaşma Senaryosu
            if (msg.Contains("merhaba") || msg.Contains("selam") || msg.Contains("mrb"))
            {
                return Json(new { reply = "Merhaba! Ben RealEstate Akıllı Yapay Zeka Asistanıyım. Platformdaki ilanları sorgulayabilir, benden emlak veya vasıta önerisi alabilirsiniz. <br/><br/>Bana <strong>'en ucuz ilanlar'</strong> veya <strong>'İstanbul ilanları'</strong> gibi sorular sorabilirsiniz!" });
            }

            // 2. Fiyat Bazlı Ucuz İlan Sorgusu
            if (msg.Contains("en ucuz") || msg.Contains("uygun fiyat") || msg.Contains("fiyatı düşük"))
            {
                var cheapest = listings.OrderBy(x => x.Price).Take(3).ToList();
                if (!cheapest.Any())
                {
                    return Json(new { reply = "Şu anda sistemde yayında olan uygun fiyatlı ilan bulunmuyor." });
                }

                var replyText = "Sistemdeki <strong>en uygun fiyatlı 3 ilanı</strong> sizin için buldum:<br/><br/>";
                foreach (var item in cheapest)
                {
                    replyText += $"⚡ <strong><a href='/Listing/Detail/{item.Id}' target='_blank' style='text-decoration:none; color:#2563eb;'>{item.Title}</a></strong><br/>" +
                                 $"💵 Fiyat: <strong style='color:#10b981;'>{item.Price:N0} ₺</strong><br/><br/>";
                }
                return Json(new { reply = replyText });
            }

            // 3. Fiyat Bazlı Pahalı/Lüks İlan Sorgusu
            if (msg.Contains("en pahalı") || msg.Contains("lüks") || msg.Contains("villa") || msg.Contains("pahali"))
            {
                var premium = listings.OrderByDescending(x => x.Price).Take(3).ToList();
                if (!premium.Any())
                {
                    return Json(new { reply = "Şu anda sistemde lüks ilan bulunmuyor." });
                }

                var replyText = "Sistemdeki öne çıkan <strong>lüks/prestijli 3 ilanımız</strong>:<br/><br/>";
                foreach (var item in premium)
                {
                    replyText += $"💎 <strong><a href='/Listing/Detail/{item.Id}' target='_blank' style='text-decoration:none; color:#2563eb;'>{item.Title}</a></strong><br/>" +
                                 $"💵 Fiyat: <strong style='color:#10b981;'>{item.Price:N0} ₺</strong><br/><br/>";
                }
                return Json(new { reply = replyText });
            }

            // 4. Kategoriye Göre İlan Arama
            foreach (var cat in categories)
            {
                var catNameLower = cat.Name.ToLower(new System.Globalization.CultureInfo("tr-TR"));
                if (msg.Contains(catNameLower))
                {
                    var catListings = listings.Where(x => x.CategoryId == cat.Id).Take(3).ToList();
                    if (!catListings.Any())
                    {
                        return Json(new { reply = $"Şu anda <strong>{cat.Name}</strong> kategorisinde aktif ilan bulunmuyor." });
                    }

                    var replyText = $"<strong>{cat.Name}</strong> kategorisindeki güncel ilanlar:<br/><br/>";
                    foreach (var item in catListings)
                    {
                        replyText += $"🏢 <strong><a href='/Listing/Detail/{item.Id}' target='_blank' style='text-decoration:none; color:#2563eb;'>{item.Title}</a></strong><br/>" +
                                     $"💵 Fiyat: <strong style='color:#10b981;'>{item.Price:N0} ₺</strong><br/><br/>";
                    }
                    return Json(new { reply = replyText });
                }
            }

            // 5. Şehre Göre İlan Arama (Lokasyon Bazlı)
            foreach (var city in cities)
            {
                var cityNameLower = city.Name.ToLower(new System.Globalization.CultureInfo("tr-TR"));
                if (msg.Contains(cityNameLower))
                {
                    var cityListings = listings.Where(x => x.CityId == city.Id).Take(3).ToList();
                    if (!cityListings.Any())
                    {
                        return Json(new { reply = $"Şu anda <strong>{city.Name}</strong> şehrinde yayında olan ilanımız bulunmuyor." });
                    }

                    var replyText = $"📍 <strong>{city.Name}</strong> şehrindeki aktif ilanlarımız:<br/><br/>";
                    foreach (var item in cityListings)
                    {
                        replyText += $"▪ <strong><a href='/Listing/Detail/{item.Id}' target='_blank' style='text-decoration:none; color:#2563eb;'>{item.Title}</a></strong><br/>" +
                                     $"💵 Fiyat: <strong style='color:#10b981;'>{item.Price:N0} ₺</strong><br/><br/>";
                    }
                    return Json(new { reply = replyText });
                }
            }

            // 6. Başlık veya Açıklamada Genel Arama Eşlemesi
            var keywordMatches = listings
                .Where(x => x.Title.ToLower(new System.Globalization.CultureInfo("tr-TR")).Contains(msg) ||
                            x.Description.ToLower(new System.Globalization.CultureInfo("tr-TR")).Contains(msg))
                .Take(3)
                .ToList();

            if (keywordMatches.Any())
            {
                var replyText = "Arama kriterlerinize benzeyen bazı ilanları listeledim:<br/><br/>";
                foreach (var item in keywordMatches)
                {
                    replyText += $"🔍 <strong><a href='/Listing/Detail/{item.Id}' target='_blank' style='text-decoration:none; color:#2563eb;'>{item.Title}</a></strong><br/>" +
                                 $"💵 Fiyat: <strong style='color:#10b981;'>{item.Price:N0} ₺</strong><br/><br/>";
                }
                return Json(new { reply = replyText });
            }

            // 7. Eşleşme Bulunamadığında Fallback (Dostane Yanıt)
            return Json(new { reply = "Aradığınız kriterlere uygun bir ilan bulamadım. Ancak bana <strong>'en ucuz'</strong>, <strong>'İstanbul'</strong> veya <strong>'Ev'</strong> gibi anahtar kelimeler yazarak arama yapabilirsiniz." });
        }
    }
}