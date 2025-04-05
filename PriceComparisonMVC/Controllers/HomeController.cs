using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TokenManager _tokenManager;
        private readonly IApiService _apiService;

        public HomeController(ILogger<HomeController> logger, TokenManager tokenManager, IApiService apiService)
        {
            _logger = logger;
            _tokenManager = tokenManager;
            _apiService = apiService;
        }

        //public async Task<IActionResult> IndexAsync()
        //{
        //    var indexContent = Data.IndexContentData.GetIndexContent();

        //    return View(indexContent);
        //}

        public async Task<IActionResult> IndexAsync(string category = null)
        {
            var indexContent = Data.IndexContentData.GetIndexContent();

            // Якщо вказана категорія та вона існує в словнику, змінюємо товари
            if (!string.IsNullOrEmpty(category) && indexContent.ProductsByCategory.ContainsKey(category))
            {
                indexContent.SelectedCategory = category;
                indexContent.PopularProducts = indexContent.ProductsByCategory[category];
            }

            return View(indexContent);
        }

        // Метод для переходу на сторінку деталей продукту
        public IActionResult ProductDetails(string id)
        {
            // Тут повинна бути логіка для отримання деталей продукту
            // Поки просто перенаправляємо на відповідний контролер
            return RedirectToAction("Details", "Products", new { id });
        }

        [HttpGet]
        public IActionResult GetProductsByCategory(string category)
        {
            var indexContent = Data.IndexContentData.GetIndexContent();

            if (!string.IsNullOrEmpty(category) && indexContent.ProductsByCategory.ContainsKey(category))
            {
                // Повертаємо лише товари вибраної категорії
                return Json(indexContent.ProductsByCategory[category]);
            }

            // Якщо категорія не вказана або не існує, повертаємо смартфони за замовчуванням
            return Json(indexContent.ProductsByCategory["Смартфони"]);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
