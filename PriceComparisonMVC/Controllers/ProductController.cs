using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Product;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;
using System.Text.Json;

namespace PriceComparisonMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int firstProductId, int feedbackPage = 1)
        {
            try
            {
                var productPageModel = await _productService.GetProductPageModelAsync(firstProductId, feedbackPage);

                // Додаємо запис про переглянутий товар у куки
                AddToRecentlyViewed(firstProductId, productPageModel.CategoryId);

                //відображення блоку категорій
                var categories = Data.IndexContentData.GetCategories();
                ViewBag.Categories = categories;

                return View(productPageModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Загальна помилка: {ex.Message}");
                return View(new ProductPageModel
                {
                    ProductResponseModel = new ProductResponseModel
                    {
                        Title = "Виникла помилка",
                        Description = "Не вдалося завантажити інформацію про товар",
                        Brand = "Помилка"
                    }
                });
            }
        }

        // Додатковий метод для додавання відгуків (розкоментований з оригінального коду)
        //[HttpPost]
        //public async Task<IActionResult> AddFeedback(FeedbackRequestModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["FeedbackError"] = "Будь ласка, перевірте правильність заповнення всіх полів";
        //        return RedirectToAction("Index", new { id = model.ProductId });
        //    }
        //
        //    try
        //    {
        //        await _productService.AddFeedbackAsync(model);
        //        TempData["FeedbackSuccess"] = "Ваш відгук успішно додано!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["FeedbackError"] = "Не вдалося додати відгук. Спробуйте пізніше.";
        //        Console.WriteLine($"Помилка додавання відгуку: {ex.Message}");
        //    }
        //
        //    return RedirectToAction("Index", new { id = model.ProductId });
        //}


        // Метод для додавання товару в історію переглядів
        private void AddToRecentlyViewed(int productId, int categoryId)
        {
            try
            {
                // Отримуємо список переглянутих товарів з куків
                string viewedCookie = Request.Cookies["recently_viewed_products"];
                List<ViewedProductItem> viewedItems = new List<ViewedProductItem>();

                if (!string.IsNullOrEmpty(viewedCookie))
                {
                    try
                    {
                        // Десеріалізуємо дані з куків
                        viewedItems = JsonSerializer.Deserialize<List<ViewedProductItem>>(viewedCookie);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка при десеріалізації переглянутих товарів: {ex.Message}");
                        viewedItems = new List<ViewedProductItem>();
                    }
                }

                // Перевіряємо, чи товар вже є в списку переглянутих
                var existingItem = viewedItems.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    // Якщо товар вже є у списку, оновлюємо час перегляду і переміщаємо на початок
                    viewedItems.Remove(existingItem);
                    existingItem.ViewedAt = DateTime.Now;
                    viewedItems.Insert(0, existingItem);
                }
                else
                {
                    // Додаємо новий запис про переглянутий товар
                    viewedItems.Insert(0, new ViewedProductItem
                    {
                        ProductId = productId,
                        CategoryId = categoryId,
                        ViewedAt = DateTime.Now
                    });

                    // Обмежуємо кількість записів у списку (максимум 15)
                    if (viewedItems.Count > 15)
                    {
                        viewedItems = viewedItems.Take(15).ToList();
                    }
                }

                // Зберігаємо оновлений список у куках
                Response.Cookies.Append("recently_viewed_products", JsonSerializer.Serialize(viewedItems), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при додаванні товару в історію переглядів: {ex.Message}");
            }
        }

        public class ViewedProductItem
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public DateTime ViewedAt { get; set; }
        }

    }
}