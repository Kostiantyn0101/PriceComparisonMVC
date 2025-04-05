using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Data;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;
using System.Reflection;
using System.Text.Json;

namespace PriceComparisonMVC.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IApiService _apiService;

        public FavoritesController(ICategoryService categoryService, IApiService apiService)
        {
            _categoryService = categoryService;
            _apiService = apiService;
        }

        public async Task<IActionResult> Favorites()
        {
            try
            {
                // Отримуємо список обраних товарів з куків
                string favoritesCookie = Request.Cookies["favorite_products"];

                // Створюємо модель для перегляду
                var model = new FavoritesViewModel
                {
                    CategoryGroups = new List<CategoryWithFavoriteProducts>()
                };

                if (!string.IsNullOrEmpty(favoritesCookie))
                {
                    List<FavoriteItem> favoriteItems = null;

                    try
                    {
                        // Спроба десеріалізувати дані як масив об'єктів (новий формат)
                        favoriteItems = JsonSerializer.Deserialize<List<FavoriteItem>>(favoritesCookie);
                    }
                    catch (JsonException)
                    {
                        // Якщо виникла помилка JSON, спробуємо десеріалізувати як масив чисел (старий формат)
                        try
                        {
                            var oldFormatIds = JsonSerializer.Deserialize<List<int>>(favoritesCookie);

                            if (oldFormatIds != null && oldFormatIds.Any())
                            {
                                // Конвертуємо старий формат у новий (припускаємо, що всі ID належать до категорії 1 як запасний варіант)
                                favoriteItems = oldFormatIds.Select(id => new FavoriteItem { productId = id, categoryId = 1 }).ToList();

                                // Оновлюємо кукі до нового формату
                                Response.Cookies.Append("favorite_products", JsonSerializer.Serialize(favoriteItems), new CookieOptions
                                {
                                    Expires = DateTime.Now.AddDays(30)
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка при обробці кукі у старому форматі: {ex.Message}");
                        }
                    }

                    if (favoriteItems != null && favoriteItems.Any())
                    {
                        // Групуємо товари за categoryId
                        var groupedByCategory = favoriteItems.GroupBy(f => f.categoryId);

                        // Для кожної категорії отримуємо інформацію про категорію та товари
                        foreach (var group in groupedByCategory)
                        {
                            int categoryId = group.Key;

                            // Отримуємо інформацію про категорію
                            var categoryInfo = await _categoryService.GetCategoryInfoAsync(categoryId);

                            if (categoryInfo != null)
                            {
                                var categoryWithProducts = new CategoryWithFavoriteProducts
                                {
                                    Category = categoryInfo,
                                    Products = new List<ProductToCategoriesListModel>()
                                };

                                // Для кожного товару отримуємо інформацію
                                foreach (var item in group)
                                {
                                    // Використовуємо існуючий метод для отримання списку продуктів категорії
                                    var productsInCategory = await _categoryService.GetProductsByCategoryAsync(categoryId);

                                    // Знаходимо потрібний продукт у списку
                                    var productInfo = productsInCategory.FirstOrDefault(p => p.Id == item.productId || p.FirstProductId == item.productId);

                                    if (productInfo != null)
                                    {
                                        categoryWithProducts.Products.Add(productInfo);
                                    }
                                }

                                if (categoryWithProducts.Products.Any())
                                {
                                    model.CategoryGroups.Add(categoryWithProducts);
                                }
                            }
                        }
                    }
                }

                // Отримуємо дані категорій для верхнього меню
                var categories = Data.IndexContentData.GetCategories();
                ViewBag.Categories = categories;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Сталася помилка при отриманні обраних товарів: " + ex.Message;
                return View(new FavoritesViewModel { CategoryGroups = new List<CategoryWithFavoriteProducts>() });
            }
        }

        public IActionResult RemoveFromFavorites(int productId)
        {
            try
            {
                // Отримуємо список обраних товарів з куків
                string favoritesCookie = Request.Cookies["favorite_products"];

                if (!string.IsNullOrEmpty(favoritesCookie))
                {
                    // Десеріалізуємо JSON з куків
                    var favoriteItems = JsonSerializer.Deserialize<List<FavoriteItem>>(favoritesCookie);

                    if (favoriteItems != null)
                    {
                        // Видаляємо товар зі списку
                        favoriteItems.RemoveAll(f => f.productId == productId);

                        // Оновлюємо кукі
                        Response.Cookies.Append("favorite_products", JsonSerializer.Serialize(favoriteItems), new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30)
                        });
                    }
                }

                return RedirectToAction("Favorites");
                //return Favorites();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Сталася помилка при видаленні товару з обраних: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }

    // Допоміжні класи для роботи з обраними товарами
    public class FavoriteItem
    {
        public int productId { get; set; }
        public int categoryId { get; set; }
    }

    public class FavoritesViewModel
    {
        public List<CategoryWithFavoriteProducts> CategoryGroups { get; set; }
    }

    public class CategoryWithFavoriteProducts
    {
        public CategoryResponseModel Category { get; set; }
        public List<ProductToCategoriesListModel> Products { get; set; }
    }
}