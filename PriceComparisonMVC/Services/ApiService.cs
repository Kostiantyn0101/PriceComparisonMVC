using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace PriceComparisonMVC.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenManager _tokenManager;

        public ApiService(HttpClient httpClient, TokenManager tokenManager)
        {
            _httpClient = httpClient;
            _tokenManager = tokenManager;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            return await ExecuteRequestAsync<T>(() => _httpClient.GetAsync(endpoint));
        }


        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
        {
            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            return await ExecuteRequestAsync<TResponse>(() => _httpClient.PostAsync(endpoint, content));
        }


        public async Task PostAsync<TRequest>(string endpoint, TRequest requestData)
        {
            try
            {
                SetAuthorizationHeader();
                var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newToken = await _tokenManager.RefreshTokenAsync();
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        SetAuthorizationHeader();
                        response = await _httpClient.PostAsync(endpoint, content);
                    }
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during API request: {ex.Message}");
            }
        }


        private async Task<T> ExecuteRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await requestFunc();

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var newToken = await _tokenManager.RefreshTokenAsync();
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        SetAuthorizationHeader();
                        response = await requestFunc();
                    }
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during API request: {ex.Message}");
            }
        }

        private void SetAuthorizationHeader()
        {
            var token = _tokenManager.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}