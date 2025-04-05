using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Models.User;
using PriceComparisonMVC.Services;
using PriceComparisonMVC.Models.Request;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace PriceComparisonMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly TokenManager _tokenManager;
        private readonly ICategoryService _categoryService;

        public AccountController(IAuthService authService, TokenManager tokenManager,  ICategoryService categoryService)
        {
            _authService = authService;
            _tokenManager = tokenManager;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginResponseModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginResponseModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
                return View(model);
            }

            var isSuccess = await _authService.LoginAsync(model);

            if (!isSuccess)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Невірний логін чи пароль" });
                }
                ModelState.AddModelError(string.Empty, "Невірний логін чи пароль");
                return View(model);
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public IActionResult Logout()
        {
            _tokenManager.ClearToken();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = string.Join(", ", errors) });
                }
                return View(model);
            }

            var errorMessage = await _authService.RegisterAsync(model);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = errorMessage });
                }
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, redirectUrl = Url.Action("Login", "Account") });
            }
            return RedirectToAction("Login", "Account");
        }


        //[Authorize]
        //public async Task<IActionResult> Profile()
        //{
        //    try
        //    {

        //        string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        //        var userProfile = new UserProfileModel
        //        {
        //            Username = username,
        //            Email = email,
        //            AvatarUrl = "/images/empty-avatar.jpeg",
        //            RegistrationDate = DateTime.Now.AddMonths(-6),
        //            ReviewsCount = 12
        //        };

        //        return View(userProfile);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Якщо сталася помилка, показуємо базовий профіль з даними з Identity
        //        var basicProfile = new UserProfileModel
        //        {
        //            Username = User.Identity.Name,
        //            Email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Не вказано",
        //            AvatarUrl = "/images/default-avatar.png"
        //        };

        //        return View(basicProfile);
        //    }
        //}

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                // Отримання основних даних користувача
                string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var userProfile = new UserProfileModel
                {
                    Username = username,
                    Email = email,
                    AvatarUrl = "/images/empty-avatar.jpeg",
                    RegistrationDate = DateTime.Now.AddMonths(-6),
                    ReviewsCount = 12
                };

                // Отримання переглянутих товарів
                await LoadRecentlyViewedProductsAsync(userProfile);

                // Отримання обраних товарів
                await LoadFavoriteProductsAsync(userProfile);

                // Отримання товарів для порівняння
                await LoadComparisonProductsAsync(userProfile);

                return View(userProfile);
            }
            catch (Exception ex)
            {
                // Якщо сталася помилка, показуємо базовий профіль з даними з Identity
                var basicProfile = new UserProfileModel
                {
                    Username = User.Identity.Name,
                    Email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Не вказано",
                    AvatarUrl = "/images/default-avatar.png"
                };
                return View(basicProfile);
            }
        }


        private async Task LoadRecentlyViewedProductsAsync(UserProfileModel userProfile)
        {
            // Отримуємо список переглянутих товарів з куків
            string viewedCookie = Request.Cookies["recently_viewed_products"];

            if (!string.IsNullOrEmpty(viewedCookie))
            {
                List<ViewedProductItem> viewedItems = null;

                try
                {
                    // Десеріалізуємо дані з куків
                    viewedItems = JsonSerializer.Deserialize<List<ViewedProductItem>>(viewedCookie);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при десеріалізації переглянутих товарів: {ex.Message}");
                    return;
                }

                if (viewedItems != null && viewedItems.Any())
                {
                    // Групуємо товари за categoryId
                    var groupedByCategory = viewedItems
                        .GroupBy(v => v.CategoryId)
                        .OrderByDescending(g => g.Max(v => v.ViewedAt)); // Сортуємо за часом останнього перегляду

                    // Для кожної категорії отримуємо інформацію про категорію та товари
                    foreach (var group in groupedByCategory)
                    {
                        int categoryId = group.Key;

                        // Отримуємо інформацію про категорію
                        var categoryInfo = await _categoryService.GetCategoryInfoAsync(categoryId);

                        if (categoryInfo != null)
                        {
                            var categoryWithProducts = new CategoryWithProducts
                            {
                                Category = categoryInfo
                            };

                            // Для кожного товару отримуємо інформацію
                            foreach (var item in group.OrderByDescending(v => v.ViewedAt))
                            {
                                // Використовуємо існуючий метод для отримання списку продуктів категорії
                                var productsInCategory = await _categoryService.GetProductsByCategoryAsync(categoryId);

                                // Знаходимо потрібний продукт у списку
                                var productInfo = productsInCategory.FirstOrDefault(p => p.Id == item.ProductId || p.FirstProductId == item.ProductId);

                                if (productInfo != null)
                                {
                                    categoryWithProducts.Products.Add(productInfo);
                                }
                            }

                            if (categoryWithProducts.Products.Any())
                            {
                                userProfile.RecentlyViewedCategoryGroups.Add(categoryWithProducts);
                            }
                        }
                    }
                }
            }
        }

        private async Task LoadFavoriteProductsAsync(UserProfileModel userProfile)
        {
            // Отримуємо список обраних товарів з куків
            string favoritesCookie = Request.Cookies["favorite_products"];

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
                            // Конвертуємо старий формат у новий (припускаємо, що всі ID належать до категорії 1)
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
                        Console.WriteLine($"Помилка при обробці кукі обраних товарів у старому форматі: {ex.Message}");
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
                            var categoryWithProducts = new CategoryWithProducts
                            {
                                Category = categoryInfo
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
                                userProfile.FavoriteCategoryGroups.Add(categoryWithProducts);
                            }
                        }
                    }
                }
            }
        }

        private async Task LoadComparisonProductsAsync(UserProfileModel userProfile)
        {
            // Отримуємо список товарів для порівняння з куків
            string comparisonCookie = Request.Cookies["comparison_products"];

            if (!string.IsNullOrEmpty(comparisonCookie))
            {
                List<ComparisonItem> comparisonItems = null;

                try
                {
                    // Спроба десеріалізувати дані як масив об'єктів
                    comparisonItems = JsonSerializer.Deserialize<List<ComparisonItem>>(comparisonCookie);
                }
                catch (JsonException)
                {
                    // Якщо виникла помилка JSON, спробуємо десеріалізувати як масив чисел (старий формат)
                    try
                    {
                        var oldFormatIds = JsonSerializer.Deserialize<List<int>>(comparisonCookie);

                        if (oldFormatIds != null && oldFormatIds.Any())
                        {
                            // Конвертуємо старий формат у новий (припускаємо, що всі ID належать до категорії 1)
                            comparisonItems = oldFormatIds.Select(id => new ComparisonItem { productId = id, categoryId = 1 }).ToList();

                            // Оновлюємо кукі до нового формату
                            Response.Cookies.Append("comparison_products", JsonSerializer.Serialize(comparisonItems), new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка при обробці кукі порівнянь у старому форматі: {ex.Message}");
                    }
                }

                if (comparisonItems != null && comparisonItems.Any())
                {
                    // Групуємо товари за categoryId
                    var groupedByCategory = comparisonItems.GroupBy(f => f.categoryId);

                    // Для кожної категорії отримуємо інформацію про категорію та товари
                    foreach (var group in groupedByCategory)
                    {
                        int categoryId = group.Key;

                        // Отримуємо інформацію про категорію
                        var categoryInfo = await _categoryService.GetCategoryInfoAsync(categoryId);

                        if (categoryInfo != null)
                        {
                            var categoryWithProducts = new CategoryWithProducts
                            {
                                Category = categoryInfo
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
                                userProfile.ComparisonCategoryGroups.Add(categoryWithProducts);
                            }
                        }
                    }
                }
            }
        }
    }

    // Класи для десеріалізації даних з куків
    //public class FavoriteItem
    //{
    //    public int productId { get; set; }
    //    public int categoryId { get; set; }
    //}

    public class ComparisonItem
    {
        public int productId { get; set; }
        public int categoryId { get; set; }
    }

    public class ViewedProductItem
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}



