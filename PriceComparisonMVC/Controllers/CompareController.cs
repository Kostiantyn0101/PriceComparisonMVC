using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonMVC.Services;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Models.Compare;
using PriceComparisonMVC.Models;
using System.Buffers.Text;
using Microsoft.Extensions.Caching.Memory;

namespace PriceComparisonMVC.Controllers
{
    public class CompareController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IMemoryCache _memoryCache;

        // Список ідентифікаторів товарів для порівняння (тимчасове рішення без сесій)
        private static List<int> _comparisonProducts = new List<int>();

        public CompareController(IApiService apiService, IMemoryCache memoryCache)
        {
            _apiService = apiService;
            _memoryCache = memoryCache;
        }

        // GET: /Compare/Comparison
        public async Task<IActionResult> Comparison()
        {

            // Отримуємо дані категорій (з того ж джерела, що й для головної сторінки)
            var categories = Data.IndexContentData.GetCategories();

            // Передаємо через ViewBag
            ViewBag.Categories = categories;

            var productIds = GetComparisonProductsFromCookies();
            

            if (productIds.Count == 0)
            {
                return View(new List<ProductComparisonViewModel>());
            }

            var comparisonProducts = new List<ProductComparisonViewModel>();

            foreach (var productId in productIds)
            {
                try
                {
                    // Отримуємо основну інформацію про товар
                    var productResponseModel = await _apiService.GetAsync<ProductNewResponseModel>($"api/Products/{productId}");

                    // Отримуємо інформацію про базовий продукт (бренд і назву)
                    var baseProductResponseModel = await _apiService.GetAsync<BaseProductResponseModel>($"api/BaseProducts/{productResponseModel.BaseProductId}");

                    // Отримуємо характеристики товару
                    var characteristics = await _apiService.GetAsync<List<ProductCharacteristicResponseModel>>($"api/ProductCharacteristics/{productId}");

                    // Отримуємо зображення товару
                    var productImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{productId}");

                    // Отримуємо дані продавців для визначення цінового діапазону
                    List<SellerProductDetailResponseModel> sellerProductDetails = null;
                    try
                    {
                        sellerProductDetails = await _apiService.GetAsync<List<SellerProductDetailResponseModel>>($"api/SellerProductDetails/{productId}");
                    }
                    catch (Exception sellerEx)
                    {
                        Console.WriteLine($"Помилка при отриманні даних продавців для товару {productId}: {sellerEx.Message}");
                        sellerProductDetails = new List<SellerProductDetailResponseModel>();
                    }

                    // Визначаємо мінімальну та максимальну ціну, а також кількість пропозицій
                    decimal minPrice = 0;
                    decimal maxPrice = 0;
                    int offerCount = 0;
                    bool hasOffers = false;

                    if (sellerProductDetails != null && sellerProductDetails.Any())
                    {
                        minPrice = sellerProductDetails.Min(s => s.PriceValue);
                        maxPrice = sellerProductDetails.Max(s => s.PriceValue);
                        offerCount = sellerProductDetails.Count;
                        hasOffers = true;
                    }

                    // Формуємо модель представлення для порівняння
                    var comparisonModel = new ProductComparisonViewModel
                    {
                        Id = productId,
                        Name = baseProductResponseModel?.Title ?? "Невідомий товар",
                        Brand = baseProductResponseModel?.Brand ?? "Невідомий бренд",
                        Model = productResponseModel?.ModelNumber ?? "",
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        OfferCount = offerCount,
                        HasOffers = hasOffers,
                        ImageUrl = productImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
                        Specifications = ConvertCharacteristicsToSpecifications(characteristics)
                    };

                    comparisonProducts.Add(comparisonModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при отриманні даних для товару {productId}: {ex.Message}");
                }
            }

            // Сортуємо товари за мінімальною ціною
            comparisonProducts = comparisonProducts.OrderBy(p => p.MinPrice).ToList();

            return View(comparisonProducts);
        }


        // GET: /Compare/CategoryProducts?id={productId}
        public async Task<IActionResult> CategoryProducts(int id)
        {

            var categoryDetails = await _apiService.GetAsync<CategoryDetailsResponseModel>($"/api/Categories/getbyproduct/{id}");

            if (categoryDetails == null)
            {
                TempData["ErrorMessage"] = "Не вдалося отримати дані категорії для даного продукту";
                return RedirectToAction("Comparison", "Compare");
            }

            // Припускаємо, що модель CategoryDetailsResponseModel містить властивість CategoryId (якщо інша – змініть відповідно)
            return RedirectToAction("CategoryProductList", "Categories", new { id = categoryDetails.Id });
        }



        // Helper метод для конвертації характеристик у словник специфікацій
        private Dictionary<string, string> ConvertCharacteristicsToSpecifications(List<ProductCharacteristicResponseModel> characteristics)
        {
            var specifications = new Dictionary<string, string>();

            if (characteristics == null)
                return specifications;

            foreach (var characteristic in characteristics)
            {
                string value = "";

                switch (characteristic.CharacteristicDataType.ToLower())
                {
                    case "decimal":
                        value = characteristic.ValueNumber?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(characteristic.CharacteristicUnit))
                        {
                            value += " " + characteristic.CharacteristicUnit;
                        }
                        break;
                    case "string":
                        value = characteristic.ValueText ?? "";
                        break;
                    case "bool":
                        value = characteristic.ValueBoolean.HasValue ? (characteristic.ValueBoolean.Value ? "Так" : "Ні") : "";
                        break;
                    case "datetime":
                        value = characteristic.ValueDate.HasValue ? characteristic.ValueDate.Value.ToString("yyyy-MM-dd") : "";
                        break;
                    default:
                        break;
                }

                specifications[characteristic.CharacteristicTitle] = value;
            }

            return specifications;
        }

        // GET: /Compare/Add
        public IActionResult Add(int id)
        {
            if (!_comparisonProducts.Contains(id))
            {
                if (_comparisonProducts.Count < 5)
                {
                    _comparisonProducts.Add(id);
                    TempData["SuccessMessage"] = "Товар додано до порівняння";
                }
                else
                {
                    TempData["ErrorMessage"] = "Можна порівнювати не більше 5 товарів одночасно";
                }
            }
            else
            {
                TempData["InfoMessage"] = "Цей товар вже додано до порівняння";
            }

            return RedirectToAction("Comparison");
        }


        // Метод для збереження списку товарів в куки
        private void SaveComparisonProductsToCookies(List<int> products)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                Path = "/"
            };

            Response.Cookies.Append("comparison_products",
                System.Text.Json.JsonSerializer.Serialize(products),
                options);
        }

        //Оновлюємо методи для роботи з куками
        public IActionResult RemoveFromComparison(int id)
        {
            var products = GetComparisonProductsFromCookies();

            if (products.Contains(id))
            {
                products.Remove(id);
                SaveComparisonProductsToCookies(products);
                TempData["InfoMessage"] = "Товар видалено з порівняння";
            }

            return RedirectToAction("Comparison");
        }

        public IActionResult ClearComparison()
        {
            // Видаляємо куку
            Response.Cookies.Delete("comparison_products");
            TempData["InfoMessage"] = "Список порівняння очищено";

            return RedirectToAction("Comparison");
        }




        //Сторінка анімації 
        // GET: /Compare/SmartComparison?productIdA=1&productIdB=2
        public IActionResult SmartComparison(int productIdA, int productIdB)
        {
            // Перенаправляємо на сторінку завантаження
            return View("SmartComparisonLoading");
        }



        [HttpGet]
        public IActionResult CheckComparisonStatus()
        {
            bool isComplete =
                TempData.ContainsKey("ComparisonComplete") ||
                TempData.ContainsKey("ErrorMessage");

            // Зберігаємо значення TempData для наступного запиту
            if (TempData.ContainsKey("ComparisonComplete"))
                TempData.Keep("ComparisonComplete");

            if (TempData.ContainsKey("ComparisonCacheKey"))
                TempData.Keep("ComparisonCacheKey");

            if (TempData.ContainsKey("ErrorMessage"))
                TempData.Keep("ErrorMessage");

            return Json(new { isComplete });
        }



        public async Task<IActionResult> SmartComparisonData(int productIdA, int productIdB)
        {
            try
            {
                // Отримуємо дані порівняння з API
                var apiResponse = await _apiService.GetAsync<ComparisonApiResponse>(
                    $"api/ProductComparison/comparegpt?productIdA={productIdA}&productIdB={productIdB}");

                // Створюємо унікальний ключ для кешу
                string cacheKey = $"comparison_{productIdA}_{productIdB}";

                // Зберігаємо результат у кеші з часом життя 30 хвилин
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2));

                _memoryCache.Set(cacheKey, apiResponse, cacheOptions);

                // У TempData зберігаємо лише ключ кешу та ознаку завершення
                TempData["ComparisonCacheKey"] = cacheKey;
                TempData["ComparisonComplete"] = true;

                return Ok();
            }
            catch (Exception ex)
            {
                // Зберігаємо помилку в TempData
                TempData["ErrorMessage"] = $"Помилка при отриманні даних порівняння: {ex.Message}";
                TempData["ComparisonComplete"] = true;
                return BadRequest(ex.Message);
            }
        }



        public async Task<IActionResult> SmartComparisonResult(int productIdA, int productIdB)
        {
            try
            {

                // Отримуємо дані категорій (з того ж джерела, що й для головної сторінки)
                var categories = Data.IndexContentData.GetCategories();

                // Передаємо через ViewBag
                ViewBag.Categories = categories;

                ComparisonApiResponse apiResponse = null;

                // Спробуйте отримати ключ кешу з TempData
                if (TempData.ContainsKey("ComparisonCacheKey"))
                {
                    string cacheKey = TempData["ComparisonCacheKey"].ToString();

                    // Отримуємо дані з кешу
                    if (_memoryCache.TryGetValue(cacheKey, out ComparisonApiResponse cachedResponse))
                    {
                        apiResponse = cachedResponse;
                    }
                }

                // Якщо даних немає в кеші
                if (apiResponse == null)
                {
                    // Перевіряємо, чи є повідомлення про помилку
                    if (TempData.ContainsKey("ErrorMessage"))
                    {
                        // Якщо є, повертаємо на сторінку порівняння з повідомленням про помилку
                        var errorMsg = TempData["ErrorMessage"].ToString();
                        TempData["ErrorMessage"] = errorMsg; // Повторно зберігаємо для наступного запиту
                        return RedirectToAction("Comparison");
                    }

                    // Якщо немає ні даних, ні помилки - повертаємо на сторінку очікування
                    return View("SmartComparisonLoading");
                }

                // Отримуємо зображення для кожного продукту
                var productAImages = await _apiService.GetAsync<List<ProductImageModel>>(
                    $"api/ProductImage/{productIdA}");
                var productBImages = await _apiService.GetAsync<List<ProductImageModel>>(
                    $"api/ProductImage/{productIdB}");

                // Формуємо модель представлення для розумного порівняння
                var viewModel = new SmartComparisonViewModel
                {
                    ProductAName = apiResponse.ProductATitle,
                    ProductBName = apiResponse.ProductBTitle,
                    ProductAImageUrl = productAImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
                    ProductBImageUrl = productBImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
                    Explanation = apiResponse.Explanation,
                    KeyDifferences = GenerateKeyDifferences(apiResponse)
                };

                return View("SmartComparison", viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при отриманні даних порівняння: {ex.Message}";
                return RedirectToAction("Comparison");
            }
        }




        // Допоміжний метод для генерації ключових відмінностей з відповіді API
        private List<KeyDifference> GenerateKeyDifferences(ComparisonApiResponse apiResponse)
        {
            var differences = new List<KeyDifference>();

            // Перебираємо групи характеристик продукту A
            foreach (var group in apiResponse.ProductA)
            {
                var groupTitle = group.CharacteristicGroupTitle;

                foreach (var characteristic in group.ProductCharacteristics)
                {
                    if (characteristic.IsHighlighted)
                    {
                        var productBGroup = apiResponse.ProductB.FirstOrDefault(g => g.CharacteristicGroupTitle == groupTitle);
                        var productBCharacteristic = productBGroup?.ProductCharacteristics.FirstOrDefault(c => c.CharacteristicTitle == characteristic.CharacteristicTitle);

                        if (productBCharacteristic != null)
                        {
                            differences.Add(new KeyDifference
                            {
                                CharacteristicName = characteristic.CharacteristicTitle,
                                ProductAValue = characteristic.Value,
                                ProductBValue = productBCharacteristic.Value,
                                Winner = characteristic.IsHighlighted && !productBCharacteristic.IsHighlighted ? "A" :
                                         !characteristic.IsHighlighted && productBCharacteristic.IsHighlighted ? "B" : "Рівно"
                            });
                        }
                    }
                }
            }

            // Додатково перевіряємо характеристики, які виділені в продукті B, але не були враховані
            foreach (var group in apiResponse.ProductB)
            {
                var groupTitle = group.CharacteristicGroupTitle;

                foreach (var characteristic in group.ProductCharacteristics)
                {
                    if (characteristic.IsHighlighted)
                    {
                        var existingDifference = differences.FirstOrDefault(d => d.CharacteristicName == characteristic.CharacteristicTitle);
                        if (existingDifference == null)
                        {
                            var productAGroup = apiResponse.ProductA.FirstOrDefault(g => g.CharacteristicGroupTitle == groupTitle);
                            var productACharacteristic = productAGroup?.ProductCharacteristics.FirstOrDefault(c => c.CharacteristicTitle == characteristic.CharacteristicTitle);

                            if (productACharacteristic != null)
                            {
                                differences.Add(new KeyDifference
                                {
                                    CharacteristicName = characteristic.CharacteristicTitle,
                                    ProductAValue = productACharacteristic.Value,
                                    ProductBValue = characteristic.Value,
                                    Winner = "B"
                                });
                            }
                        }
                    }
                }
            }

            return differences;
        }


        //// Метод для отримання ідентифікаторів товарів з куків
        //private List<int> GetComparisonProductsFromCookies()
        //{
        //    try
        //    {
        //        // Отримуємо рядок з куків
        //        var cookieValue = Request.Cookies["comparison_products"];

        //        // Якщо кука існує, парсимо JSON і конвертуємо в список int
        //        if (!string.IsNullOrEmpty(cookieValue))
        //        {
        //            var productIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(cookieValue);
        //            return productIds ?? new List<int>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Помилка при отриманні товарів з куків: {ex.Message}");
        //    }

        //    return new List<int>();
        //}


        private List<int> GetComparisonProductsFromCookies()
        {
            try
            {
                // Отримуємо рядок з куків
                var cookieValue = Request.Cookies["comparison_products"];
                // Якщо кука існує, парсимо JSON
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    // Спробуємо спочатку розпарсити як новий формат (масив об'єктів)
                    try
                    {
                        // Десеріалізуємо як масив об'єктів типу { productId: number, categoryId: number }
                        var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductItem>>(cookieValue);
                        if (products != null)
                        {
                            // Повертаємо тільки ідентифікатори продуктів
                            return products.Select(p => p.productId).ToList();
                        }
                    }
                    catch
                    {
                        // Якщо не вдалося розпарсити як новий формат, спробуємо старий (просто масив ID)
                        var productIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(cookieValue);
                        return productIds ?? new List<int>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні товарів з куків: {ex.Message}");
            }
            return new List<int>();
        }

        // Допоміжний клас для десеріалізації елементів з куків
        private class ProductItem
        {
            public int productId { get; set; }
            public int categoryId { get; set; }
        }



    }


   
   // Класи для десеріалізації відповіді API
    public class ComparisonApiResponse
    {
        public string ProductATitle { get; set; }
        public string ProductBTitle { get; set; }
        public List<CharacteristicGroup> ProductA { get; set; }
        public List<CharacteristicGroup> ProductB { get; set; }
        public string Explanation { get; set; }
        public string AiProvider { get; set; }
    }

    public class CharacteristicGroup
    {
        public string CharacteristicGroupTitle { get; set; }
        public List<ProductCharacteristic> ProductCharacteristics { get; set; }
    }

    public class ProductCharacteristic
    {
        public string CharacteristicTitle { get; set; }
        public string Value { get; set; }
        public bool IsHighlighted { get; set; }
    }
}
