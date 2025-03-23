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

        public async Task<IActionResult> IndexAsync()
        {

            List<CategoryResponseModel> categoryResponses = await _apiService.GetAsync<List<CategoryResponseModel>>("api/categories/getall");

            var indexContent = Data.IndexContentData.GetIndexContent(categoryResponses);

            ViewBag.Username = HttpContext?.User?.Identity?.Name;

            return View(indexContent);
        }

        public IActionResult PC()
        {
            var products = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "ModelName", "Lenovo LOQ 15ABR9" },
                    { "ImageUrl", "/img/Lenovo LOQ 15ABR9.png" },
                    { "MinPrice", 28329 },
                    { "MaxPrice", 44304 },
                    { "PriceCount", 38 },
                    { "Brand", "Lenovo" },
                    { "BrandLogoUrl", "img/Frame 1707482536.png" },
                    { "ScreenSize", "15.6\"" },
                    { "MatrixType", "IPS" },
                    { "ScreenCover", "матове" },
                    { "Resolution", "1920x1080 (16:9)" },
                    { "RefreshRate", "144 Гц" },
                    { "Brightness", "300 ніт" },
                    { "Contrast", "1000:1" },
                    { "ProcessorSeries", "Ryzen 5" },
                    { "ProcessorModel", "7235HS" },
                    { "ProcessorCodename", "Rembrandt R (Zen 3+)" },
                    { "RamSize", "16 Гб" }
                },
                new Dictionary<string, object>
                {
                    { "ModelName", "Lenovo LOQ 15IAX9" },
                    { "ImageUrl", "img/Lenovo LOQ 15IAX9.jpg" },
                    { "MinPrice", 29399 },
                    { "MaxPrice", 39089 },
                    { "PriceCount", 40 },
                    { "Brand", "Lenovo" },
                    { "BrandLogoUrl", "img/Frame 1707482536.png" },
                    { "ScreenSize", "15.6\"" },
                    { "MatrixType", "IPS" },
                    { "ScreenCover", "антивідблискове" },
                    { "Resolution", "1920x1080 (16:9)" },
                    { "RefreshRate", "144 Гц" },
                    { "Brightness", "300 ніт" },
                    { "Contrast", "1000:1" },
                    { "ProcessorSeries", "Core i5" },
                    { "ProcessorModel", "12450HX" },
                    { "ProcessorCodename", "Alder Lake (12th Gen)" },
                    { "RamSize", "16 Гб" }
                },
                new Dictionary<string, object>
                {
                    { "ModelName", "Lenovo IdeaPad Gaming 3 15ACH6" },
                    { "ImageUrl", "img/Lenovo IdeaPad Gaming 3 15ACH6.jpg" },
                    { "MinPrice", 23451 },
                    { "MaxPrice", 35214 },
                    { "PriceCount", 62 },
                    { "Brand", "Lenovo" },
                    { "BrandLogoUrl", "img/Frame 1707482536.png" },
                    { "ScreenSize", "15.6\"" },
                    { "MatrixType", "IPS" },
                    { "ScreenCover", "антивідблискове" },
                    { "Resolution", "1920x1080 (16:9)" },
                    { "RefreshRate", "144 Гц" },
                    { "Brightness", "300 ніт" },
                    { "Contrast", "1000:1" },
                    { "ProcessorSeries", "Ryzen 5" },
                    { "ProcessorModel", "5500H" },
                    { "ProcessorCodename", "Cezanne (Zen 3)" },
                    { "RamSize", "16 Гб" }
                },
                new Dictionary<string, object>
                {
                    { "ModelName", "Lenovo ThinkPad L15 Gen 1 Intel" },
                    { "ImageUrl", "img/Lenovo ThinkPad L15 Gen 1 Intel.jpg" },
                    { "MinPrice", 27853 },
                    { "MaxPrice", 30249 },
                    { "PriceCount", 3 },
                    { "Brand", "Lenovo" },
                    { "BrandLogoUrl", "img/Frame 1707482536.png" },
                    { "ScreenSize", "16\"" },
                    { "MatrixType", "IPS" },
                    { "ScreenCover", "антивідблискове" },
                    { "Resolution", "1920x1200 (16:10)" },
                    { "RefreshRate", "60 Гц" },
                    { "Brightness", "300 ніт" },
                    { "Contrast", "1000:1" },
                    { "ProcessorSeries", "Core Ultra 5" },
                    { "ProcessorModel", "135U" },
                    { "ProcessorCodename", "Meteor Lake (Series 1)" },
                    { "RamSize", "32 Гб" }
                }
            };

            // Створюємо список характеристик дисплея
            var displaySpecs = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Name", "Діагональ екрана" }, { "Key", "ScreenSize" } },
                new Dictionary<string, object> { { "Name", "Тип матриці" }, { "Key", "MatrixType" } },
                new Dictionary<string, object> { { "Name", "Покриття екрана" }, { "Key", "ScreenCover" } },
                new Dictionary<string, object> { { "Name", "Роздільна здатність дисплея" }, { "Key", "Resolution" } },
                new Dictionary<string, object> { { "Name", "Частота зміни кадрів" }, { "Key", "RefreshRate" } },
                new Dictionary<string, object> { { "Name", "Яскравість" }, { "Key", "Brightness" } },
                new Dictionary<string, object> { { "Name", "Контрастність" }, { "Key", "Contrast" } }
            };

            // Створюємо список характеристик процесора
            var processorSpecs = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Name", "Серія" }, { "Key", "ProcessorSeries" } },
                new Dictionary<string, object> { { "Name", "Модель" }, { "Key", "ProcessorModel" } },
                new Dictionary<string, object> { { "Name", "Кодова назва" }, { "Key", "ProcessorCodename" } }
            };

            // Створюємо список характеристик оперативної пам'яті
            var ramSpecs = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Name", "Об'єм оперативної пам'яті" }, { "Key", "RamSize" } }
            };

            // Передаємо дані у представлення через ViewBag
            ViewBag.Products = products;
            ViewBag.DisplaySpecs = displaySpecs;
            ViewBag.ProcessorSpecs = processorSpecs;
            ViewBag.RamSpecs = ramSpecs;

            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
