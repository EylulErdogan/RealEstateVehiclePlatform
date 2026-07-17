using Microsoft.Extensions.Caching.Memory; // Namespace eklendi
using Newtonsoft.Json;
using RealEstateVehiclePlatform.WebUI.ViewModels.Auth;
using System.Net.Http.Headers;
using System.Text;

namespace RealEstateVehiclePlatform.WebUI.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache; // IMemoryCache eklendi

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache) // Constructor'a eklendi
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        public async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            // Önbelleğe alınması uygun olan metadata endpoint'leri
            var isCacheable = endpoint.Equals("Categories", StringComparison.OrdinalIgnoreCase) ||
                              endpoint.Equals("Cities", StringComparison.OrdinalIgnoreCase) ||
                              endpoint.Equals("ListingTypes", StringComparison.OrdinalIgnoreCase) ||
                              endpoint.StartsWith("Districts", StringComparison.OrdinalIgnoreCase);

            if (isCacheable)
            {
                var cacheKey = $"Cache_{endpoint.Replace('/', '_')}";
                if (_memoryCache.TryGetValue(cacheKey, out List<T>? cachedData) && cachedData != null)
                {
                    return cachedData;
                }

                var client = CreateClient();
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return new List<T>();

                var jsonData = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<T>>(jsonData)!;

                // 30 Dakika Boyunca RAM'de Sakla
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(cacheKey, data, cacheOptions);
                return data;
            }
            else
            {
                var client = CreateClient();
                var response = await client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return new List<T>();

                var jsonData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(jsonData)!;
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return default;

            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public async Task<bool> PostAsync<T>(string endpoint, T model)
        {
            EvictCache(endpoint); // Ekleme işlemi yapılırsa ilgili cache'i temizle

            var client = CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync<T>(string endpoint, T model)
        {
            EvictCache(endpoint); // Güncelleme işlemi yapılırsa ilgili cache'i temizle

            var client = CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            EvictCache(endpoint); // Silme işlemi yapılırsa ilgili cache'i temizle

            var client = CreateClient();
            var response = await client.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }

        public async Task<TokenResponseViewModel?> LoginAsync(LoginViewModel model)
        {
            var client = _httpClientFactory.CreateClient("EfApi");
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Auth/Login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponseViewModel>(result);
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("EfApi");
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<TResponse?> PostAndGetAsync<TRequest, TResponse>(
            string endpoint,
            TRequest model)
        {
            EvictCache(endpoint); // Cache invalidasyonunu tetikle

            var client = CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"API Hatası: {(int)response.StatusCode} " +
                    $"{response.StatusCode} - {responseContent}");
            }

            return JsonConvert.DeserializeObject<TResponse>(responseContent);
        }

        // Yazma işlemlerinde (POST, PUT, DELETE) önbelleği boşaltma (Cache Eviction) metodu
        private void EvictCache(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint)) return;

            if (endpoint.Contains("Categories", StringComparison.OrdinalIgnoreCase))
            {
                _memoryCache.Remove("Cache_Categories");
            }
            else if (endpoint.Contains("Cities", StringComparison.OrdinalIgnoreCase))
            {
                _memoryCache.Remove("Cache_Cities");
            }
            else if (endpoint.Contains("ListingTypes", StringComparison.OrdinalIgnoreCase))
            {
                _memoryCache.Remove("Cache_ListingTypes");
            }
            else if (endpoint.Contains("Districts", StringComparison.OrdinalIgnoreCase))
            {
                // İlçelerin tüm alt cache anahtarlarını silmek için temizleme yapabiliriz
                // (Genellikle şehir ID'sine bağlı oluşurlar: Cache_Districts_GetByCity_X)
                // En güvenli yol, ilçe listesinde bir değişim olduğunda cache'i düşürmektir.
                // MemoryCache'den tüm ilçeleri silmek için:
                var keysToRemove = new List<string>();

                // Districts ile başlayan tüm cache'leri temizlemek için:
                // Not: Districts üzerinde CRUD yapıldığında tetiklenir
                _memoryCache.Remove("Cache_Districts");
            }
        }
    }
}