using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IApiService _apiService;

        public CategoriesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        

        public async Task<IActionResult> CategoryList(int id)
        {
            var categories = await _apiService.GetAsync<List<CategoryResponseModel>>("api/categories/getall");

            // Знаходимо поточну категорію для відображення в "хлібних крихтах"
            var currentCategory = categories.FirstOrDefault(c => c.Id == id);
            ViewBag.CurrentCategory = currentCategory;

            var subcategories = categories
                .Where(c => c.ParentCategoryId.HasValue && c.ParentCategoryId.Value == id)
                .ToList();

            var categoryListModels = new List<CategoryListModel>();

            foreach (var subcategory in subcategories)
            {
                // Використовуємо виокремлений метод для отримання продуктів
                var products = await GetProductsByCategoryAsync(subcategory.Id);
                
                var topProducts = products.Take(4).ToList();

                var categoryListModel = new CategoryListModel
                {
                    ParentCategory = subcategory,
                    ProductToCategories = topProducts
                };

                categoryListModels.Add(categoryListModel);
            }

            return View(categoryListModels);
        }

        public async Task<IActionResult> CategoryProductList(int id)
        {
            try
            {
                // Отримуємо інформацію про поточну категорію для "хлібних крихт"
                var currentCategory = await _apiService.GetAsync<CategoryResponseModel>($"api/categories/{id}");
                ViewBag.CurrentCategory = currentCategory;

                // Отримуємо всі категорії, щоб знайти батьківську категорію
                var allCategories = await _apiService.GetAsync<List<CategoryResponseModel>>("api/categories/getall");

                // Встановлюємо батьківську категорію для "хлібних крихт", якщо вона є
                if (currentCategory != null && currentCategory.ParentCategoryId.HasValue)
                {
                    ViewBag.ParentCategory = allCategories.FirstOrDefault(c => c.Id == currentCategory.ParentCategoryId.Value);
                }

                // Використовуємо виокремлений метод для отримання продуктів
                var products = await GetProductsByCategoryAsync(id);

                // Створюємо моделі представлення з характеристиками для кожного продукту
                var viewModels = new List<ProductWithCharacteristicsViewModel>();
                foreach (var product in products)
                {
                    try
                    {
                        var characteristicGroups = await _apiService.GetAsync<List<ProductCharacteristicGroupResponseModel>>(
                            $"api/ProductCharacteristics/short-grouped/{product.Id}");
                        viewModels.Add(new ProductWithCharacteristicsViewModel
                        {
                            Product = product,
                            CharacteristicGroups = characteristicGroups ?? new List<ProductCharacteristicGroupResponseModel>()
                        });
                    }
                    catch (Exception ex)
                    {
                        viewModels.Add(new ProductWithCharacteristicsViewModel
                        {
                            Product = product,
                            CharacteristicGroups = new List<ProductCharacteristicGroupResponseModel>()
                        });
                    }
                }

                return View(viewModels);
            }
            catch (Exception ex)
            {
                // Обробка загальних помилок
                ViewBag.ErrorMessage = "Сталася помилка при отриманні даних: " + ex.Message;
                return View(new List<ProductWithCharacteristicsViewModel>());
            }
        }




        /// <summary>
        /// Отримує продукти за вказаною категорією
        /// </summary>
        /// <param name="categoryId">ID категорії</param>
        /// <param name="page">Номер сторінки</param>
        /// <param name="pageSize">Розмір сторінки</param>
        /// <returns>Список продуктів, конвертованих для відображення</returns>
        /// 

        private async Task<List<ProductToCategoriesListModel>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            // Отримуємо продукти за обраною категорією
            var productsWrapper = await _apiService.GetAsync<ProductsResponseWrapper>(
                $"api/Products/bycategorypaginated/{categoryId}?page={page}&pageSize={pageSize}");

            // Конвертуємо нову модель у формат, що очікується представленням
            //var products = new List<ProductToCategoriesListModel>();

            //if (productsWrapper?.Data != null)
            //{
            //    foreach (var product in productsWrapper.Data)
            //    {
            //        var convertedProduct = new ProductToCategoriesListModel
            //        {
            //            Id = product.ProductGroups.FirstOrDefault()?.FirstProductId ?? product.BaseProductId,
            //            Title = product.Title,
            //            Description = product.Description,
            //            CategoryId = product.CategoryId
            //        };

            //        try
            //        {
            //            // Все ще потрібно окремо отримувати зображення
            //            var productImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{product.BaseProductId}");
            //            convertedProduct.ImageUrl = productImages?.FirstOrDefault()?.ImageUrl;
            //        }
            //        catch (Exception ex)
            //        {
            //            convertedProduct.ImageUrl = null;
            //        }
            //        products.Add(convertedProduct);
            //    }
            //}
            //return products;


            var products = new List<ProductToCategoriesListModel>();
            if (productsWrapper?.Data != null)
            {
                foreach (var product in productsWrapper.Data)
                {
                    var firstGroup = product.ProductGroups.FirstOrDefault();
                    var convertedProduct = new ProductToCategoriesListModel
                    {
                        Id = product.BaseProductId,
                        FirstProductId = firstGroup?.FirstProductId ?? product.BaseProductId, // Зберігаємо firstProductId
                        Title = product.Title,
                        Description = product.Description,
                        CategoryId = product.CategoryId
                    };
                    try
                    {
                        // Все ще потрібно окремо отримувати зображення
                        var productImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{product.BaseProductId}");
                        convertedProduct.ImageUrl = productImages?.FirstOrDefault()?.ImageUrl;
                    }
                    catch (Exception ex)
                    {
                        convertedProduct.ImageUrl = null;
                    }
                    products.Add(convertedProduct);
                }
            }
            return products;



        }


    }
}