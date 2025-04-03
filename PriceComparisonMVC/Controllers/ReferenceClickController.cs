using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    public class ReferenceClickController : Controller
    {
        private readonly IApiService _apiService;

        public ReferenceClickController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /ReferenceClick/Track/1/2?url=https://example.com
        public async Task<IActionResult> Track(int productId, int sellerId, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL не вказаний");
            }

            try
            {
                // Отримуємо IP-адресу користувача з запиту
                string userIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";


                Console.WriteLine("\n=== Product Reference Click ===");
                Console.WriteLine($"DateTime: {DateTime.Now}");
                Console.WriteLine($"ProductId: {productId}");
                Console.WriteLine($"SellerId: {sellerId}");
                Console.WriteLine($"UserIP: {userIp}");
                Console.WriteLine($"Target URL: {url}");
                Console.WriteLine($"Request Path: {Request.Path}");
                Console.WriteLine($"User Agent: {Request.Headers["User-Agent"]}");
                Console.WriteLine("===========================\n");


                // Створюємо об'єкт для передачі в API
                var clickData = new
                {
                    productId = productId,
                    sellerId = sellerId,
                    userIp = userIp
                };

                // Відправляємо дані в API з використанням нового методу
                await _apiService.PostAsync("api/ProductReferenceClick", clickData);

                // Перенаправляємо користувача на URL магазину
                return Redirect(url);
            }
            catch (Exception ex)
            {
                // Логуємо помилку, але все одно перенаправляємо користувача
                Console.WriteLine($"Помилка при відстеженні кліку: {ex.Message}");
                return Redirect(url);
            }
        }
    }
}
