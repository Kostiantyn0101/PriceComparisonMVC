using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiService _apiService;
        private const int DEFAULT_PAGE_SIZE = 10;

        public CategoryService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<CategoryListResult> GetCategoryListAsync(int categoryId)
        {
            var result = new CategoryListResult();

            try
            {
                // Отримуємо всі категорії
                var categories = await GetAllCategoriesAsync();

                // Знаходимо поточну категорію для відображення в "хлібних крихтах"
                result.CurrentCategory = categories.FirstOrDefault(c => c.Id == categoryId);

                // Знаходимо підкатегорії для поточної категорії
                var subcategories = categories
                    .Where(c => c.ParentCategoryId.HasValue && c.ParentCategoryId.Value == categoryId)
                    .ToList();

                // Для кожної підкатегорії отримуємо топ-продукти
                foreach (var subcategory in subcategories)
                {
                    var products = await GetProductsByCategoryAsync(subcategory.Id);
                    var topProducts = products.Take(4).ToList();

                    result.CategoryListModels.Add(new CategoryListModel
                    {
                        ParentCategory = subcategory,
                        ProductToCategories = topProducts
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні списку категорій: {ex.Message}");
                throw;
            }
        }

        public async Task<CategoryProductListResult> GetCategoryProductListAsync(int categoryId)
        {
            var result = new CategoryProductListResult();

            try
            {
                // Отримуємо інформацію про поточну категорію для "хлібних крихт"
                result.CurrentCategory = await GetCategoryByIdAsync(categoryId);

                // Отримуємо всі категорії, щоб знайти батьківську категорію
                var allCategories = await GetAllCategoriesAsync();

                // Встановлюємо батьківську категорію для "хлібних крихт", якщо вона є
                if (result.CurrentCategory != null && result.CurrentCategory.ParentCategoryId.HasValue)
                {
                    result.ParentCategory = allCategories.FirstOrDefault(c =>
                        c.Id == result.CurrentCategory.ParentCategoryId.Value);
                }

                // Отримуємо продукти для поточної категорії
                var products = await GetProductsByCategoryAsync(categoryId);

                // Створюємо моделі представлення з характеристиками для кожного продукту
                foreach (var product in products)
                {
                    var productWithCharacteristics = await GetProductWithCharacteristicsAsync(product);
                    result.ProductsWithCharacteristics.Add(productWithCharacteristics);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні списку продуктів категорії: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ProductToCategoriesListModel>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            try
            {
                // Отримуємо продукти за обраною категорією
                var productsWrapper = await _apiService.GetAsync<ProductsResponseWrapper>(
                    $"api/Products/bycategorypaginated/{categoryId}?page={page}&pageSize={pageSize}");

                var products = new List<ProductToCategoriesListModel>();

                if (productsWrapper?.Data == null)
                {
                    return products;
                }

                foreach (var product in productsWrapper.Data)
                {
                    var convertedProduct = await ConvertToProductToCategoriesListModelAsync(product);
                    products.Add(convertedProduct);
                }

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні продуктів категорії {categoryId}: {ex.Message}");
                return new List<ProductToCategoriesListModel>();
            }
        }

        #region Private Helper Methods

        private async Task<List<CategoryResponseModel>> GetAllCategoriesAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<CategoryResponseModel>>("api/categories/getall");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні списку всіх категорій: {ex.Message}");
                return new List<CategoryResponseModel>();
            }
        }

        private async Task<CategoryResponseModel?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                return await _apiService.GetAsync<CategoryResponseModel>($"api/categories/{categoryId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні категорії {categoryId}: {ex.Message}");
                return null;
            }
        }

        private async Task<ProductToCategoriesListModel> ConvertToProductToCategoriesListModelAsync(dynamic product)
        {
            try
            {
                Console.WriteLine($"Конвертуємо продукт");

                // Отримання FirstProductId з ProductGroups
                int firstProductId = product.BaseProductId;
                if (product.ProductGroups != null && product.ProductGroups.Count > 0)
                {
                    // Робота з ProductGroups як з списком
                    if (product.ProductGroups.Count > 0)
                    {
                        var firstGroup = product.ProductGroups[0];
                        if (firstGroup != null && firstGroup.FirstProductId != null)
                        {
                            firstProductId = firstGroup.FirstProductId;
                        }
                    }
                }

                var convertedProduct = new ProductToCategoriesListModel
                {
                    Id = product.BaseProductId,
                    FirstProductId = firstProductId,
                    Title = product.Title ?? "Без назви",
                    Description = product.Description ?? "Без опису",
                    CategoryId = product.CategoryId
                };

                try
                {
                    var productImages = await _apiService.GetAsync<List<ProductImageModel>>(
                        $"api/ProductImage/{product.BaseProductId}");
                    convertedProduct.ImageUrl = productImages?.FirstOrDefault()?.ImageUrl;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при отриманні зображень для продукту {product.BaseProductId}: {ex.Message}");
                    convertedProduct.ImageUrl = null;
                }

                return convertedProduct;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при конвертації продукту: {ex.Message}");
                throw;
            }
        }

        private async Task<ProductWithCharacteristicsViewModel> GetProductWithCharacteristicsAsync(
            ProductToCategoriesListModel product)
        {
            try
            {
                var characteristicGroups = await _apiService.GetAsync<List<ProductCharacteristicGroupResponseModel>>(
                    $"api/ProductCharacteristics/short-grouped/{product.Id}");

                return new ProductWithCharacteristicsViewModel
                {
                    Product = product,
                    CharacteristicGroups = characteristicGroups ?? new List<ProductCharacteristicGroupResponseModel>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні характеристик для продукту {product.Id}: {ex.Message}");

                return new ProductWithCharacteristicsViewModel
                {
                    Product = product,
                    CharacteristicGroups = new List<ProductCharacteristicGroupResponseModel>()
                };
            }
        }

        // Додайте цей метод до класу CategoryService
        public async Task<CategoryResponseModel> GetCategoryInfoAsync(int categoryId)
        {
            try
            {
                return await GetCategoryByIdAsync(categoryId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні інформації про категорію {categoryId}: {ex.Message}");
                return null;
            }
        }
        #endregion
    }
}