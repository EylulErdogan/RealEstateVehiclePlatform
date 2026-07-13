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

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            var client = CreateClient();

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return new List<T>();

            var jsonData = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<T>>(jsonData)!;
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
            var client = CreateClient();

            var jsonData = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync<T>(string endpoint, T model)
        {
            var client = CreateClient();

            var jsonData = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
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

        public async Task<TResponse?> PostAndGetAsync<TRequest, TResponse>(string endpoint, TRequest model)
        {
            var client = CreateClient();

            var jsonData = JsonConvert.SerializeObject(model);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
                return default;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(result);
        }
    }
}