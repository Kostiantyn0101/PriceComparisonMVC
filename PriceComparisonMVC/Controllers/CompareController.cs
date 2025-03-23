using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonMVC.Services;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Models.Product;
using PriceComparisonMVC.Models;

namespace PriceComparisonMVC.Controllers
{
    public class CompareController : Controller
    {
        private readonly IApiService _apiService;

        // Список ідентифікаторів товарів для порівняння (тимчасове рішення без сесій)
        private static List<int> _comparisonProducts = new List<int>();

        public CompareController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Compare/Comparison
        public async Task<IActionResult> Comparison()
        {
            // Очищаємо поточний список порівняння
            _comparisonProducts.Clear();

            // Додаємо тестові ID товарів
            _comparisonProducts.Add(1);
            _comparisonProducts.Add(2);


            // Якщо список порожній, повертаємо пустий список
            if (_comparisonProducts.Count == 0)
            {
                return View(new List<ProductComparisonViewModel>());
            }

            var comparisonProducts = new List<ProductComparisonViewModel>();

            foreach (var productId in _comparisonProducts)
            {
                try
                {
                    // Отримуємо основну інформацію про товар
                    var productResponseModel = await _apiService.GetAsync<ProductResponseModel>($"api/Products/{productId}");

                    // Отримуємо характеристики товару
                    var characteristics = await _apiService.GetAsync<List<ProductCharacteristicResponseModel>>($"api/ProductCharacteristics/{productId}");

                    // Отримуємо зображення товару
                    var productImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{productId}");

                    // Отримуємо дані продавців для діапазону цін
                    var sellerProductDetails = await _apiService.GetAsync<List<SellerProductDetailResponseModel>>($"api/SellerProductDetails/{productId}");

                    // Визначаємо мінімальну та максимальну ціну
                    decimal minPrice = 0;
                    decimal maxPrice = 0;
                    int offerCount = 0;

                    if (sellerProductDetails != null && sellerProductDetails.Any())
                    {
                        minPrice = sellerProductDetails.Min(s => s.PriceValue);
                        maxPrice = sellerProductDetails.Max(s => s.PriceValue);
                        offerCount = sellerProductDetails.Count;
                    }

                    // Створюємо модель для представлення порівняння
                    var comparisonModel = new ProductComparisonViewModel
                    {
                        Id = productId,
                        Name = productResponseModel?.Title ?? "Невідомий товар",
                        Brand = productResponseModel?.Brand ?? "Невідомий бренд",
                        Model = productResponseModel?.Title ?? "",
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        OfferCount = offerCount,
                        ImageUrl = productImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
                        Specifications = ConvertCharacteristicsToSpecifications(characteristics)
                    };

                    comparisonProducts.Add(comparisonModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при отриманні даних для товару {productId}: {ex.Message}");
                    // Продовжуємо з наступним товаром
                }
            }

            // Сортуємо за мінімальною ціною
            comparisonProducts = comparisonProducts.OrderBy(p => p.MinPrice).ToList();

            return View(comparisonProducts);
        }

        // GET: /Compare/TestComparison
        public async Task<IActionResult> TestComparison()
        {
            // Очищаємо поточний список порівняння
            _comparisonProducts.Clear();

            // Додаємо тестові ID товарів
            _comparisonProducts.Add(1);
            _comparisonProducts.Add(2);

            // Перенаправляємо на звичайну сторінку порівняння, яка тепер відобразить ці товари
            return await Comparison();
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

        // GET: /Compare/Add/5
        public IActionResult Add(int id)
        {
            // Перевірка, чи товар вже у списку
            if (!_comparisonProducts.Contains(id))
            {
                // Максимальна кількість товарів для порівняння - 5
                if (_comparisonProducts.Count < 5)
                {
                    // Додати товар до списку порівняння
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

            // Повернутися на попередню сторінку або перенаправити на сторінку порівняння
            return RedirectToAction("Comparison");
        }

        // GET: /Compare/RemoveFromComparison/5
        public IActionResult RemoveFromComparison(int id)
        {
            if (_comparisonProducts.Contains(id))
            {
                _comparisonProducts.Remove(id);
                TempData["InfoMessage"] = "Товар видалено з порівняння";
            }

            return RedirectToAction("Comparison");
        }

        // GET: /Compare/ClearComparison
        public IActionResult ClearComparison()
        {
            _comparisonProducts.Clear();
            TempData["InfoMessage"] = "Список порівняння очищено";
            return RedirectToAction("Comparison");
        }
    



     // GET: /Compare/SmartComparison
        //public async Task<IActionResult> SmartComparison(int productIdA, int productIdB)
        //{
        //    try
        //    {
        //        // Отримуємо дані порівняння з API
        //        var apiResponse = await _apiService.GetAsync<SmartComparisonViewModel>($"api/ProductComparison/comparegpt?productIdA={productIdA}&productIdB={productIdB}");

        //        if (apiResponse == null)
        //        {
        //            TempData["ErrorMessage"] = "Не вдалося отримати дані порівняння.";
        //            return RedirectToAction("Comparison");
        //        }

        //        // Отримуємо додаткову інформацію про продукти
        //        var productAImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{productIdA}");
        //        var productBImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{productIdB}");

        //        // Створюємо модель представлення
        //        var viewModel = new SmartComparisonViewModel
        //        {
        //            //ProductAName = apiResponse.ProductATitle,
        //            //ProductBName = apiResponse.ProductBTitle,
        //            ProductAImageUrl = productAImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
        //            ProductBImageUrl = productBImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.jpg",
        //            Explanation = apiResponse.Explanation,
        //            //KeyDifferences = GenerateKeyDifferences(apiResponse)
        //        };

        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Помилка при отриманні даних порівняння: {ex.Message}";
        //        return RedirectToAction("Comparison");
        //    }
        //}

        // Допоміжний метод для генерації ключових відмінностей з API відповіді
        private List<KeyDifference> GenerateKeyDifferences(ComparisonApiResponse apiResponse)
        {
            var differences = new List<KeyDifference>();

            // Перебираємо групи характеристик і знаходимо виділені елементи
            foreach (var group in apiResponse.ProductA)
            {
                var groupTitle = group.CharacteristicGroupTitle;

                foreach (var characteristic in group.ProductCharacteristics)
                {
                    if (characteristic.IsHighlighted)
                    {
                        // Шукаємо відповідну характеристику в продукті B
                        var productBGroup = apiResponse.ProductB.Find(g => g.CharacteristicGroupTitle == groupTitle);
                        var productBCharacteristic = productBGroup?.ProductCharacteristics.Find(c =>
                            c.CharacteristicTitle == characteristic.CharacteristicTitle);

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

            // Перевіряємо, чи не пропустили ми характеристики, які виділені в продукті B, але не в A
            foreach (var group in apiResponse.ProductB)
            {
                var groupTitle = group.CharacteristicGroupTitle;

                foreach (var characteristic in group.ProductCharacteristics)
                {
                    if (characteristic.IsHighlighted)
                    {
                        // Перевіряємо, чи ця характеристика вже є у списку відмінностей
                        var existingDifference = differences.Find(d => d.CharacteristicName == characteristic.CharacteristicTitle);

                        if (existingDifference == null)
                        {
                            // Шукаємо відповідну характеристику в продукті A
                            var productAGroup = apiResponse.ProductA.Find(g => g.CharacteristicGroupTitle == groupTitle);
                            var productACharacteristic = productAGroup?.ProductCharacteristics.Find(c =>
                                c.CharacteristicTitle == characteristic.CharacteristicTitle);

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
    }



    // Модель представлення для товару при порівнянні
    public class ProductComparisonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int OfferCount { get; set; }
        public string ImageUrl { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
    }


    // Моделі для розумного порівняння
    public class SmartComparisonViewModel
    {
        // Інформація про перший продукт
        public string ProductAName { get; set; }
        public string ProductAImageUrl { get; set; }

        // Інформація про другий продукт
        public string ProductBName { get; set; }
        public string ProductBImageUrl { get; set; }

        // Пояснення від AI
        public string Explanation { get; set; }

        // Ключові відмінності між продуктами
        public List<KeyDifference> KeyDifferences { get; set; } = new List<KeyDifference>();
    }

    public class KeyDifference
    {
        // Назва характеристики
        public string CharacteristicName { get; set; }

        // Значення для продуктів
        public string ProductAValue { get; set; }
        public string ProductBValue { get; set; }

        // Який продукт краще за цією характеристикою (A, B, або "Рівно")
        public string Winner { get; set; }
    }

    // Класи моделей для десеріалізації відповіді API
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

