using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IApiService _apiService;

        public ProductController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int id, int firstProductId, int feedbackPage = 1)
        {
            List<ProductCharacteristicResponseModel>? characteristics = null;
            List<ProductCharacteristicGroupResponseModel>? shortCharacteristics = null;
            List<ProductImageModel>? productImages = null;
            ProductResponseModel? productResponseModel = null;
            CategoryDetailsResponseModel categoryDetails = null;
            CategoryResponseModel? category = null;
            FeedbacksPagedResponseModel? feedbacks = null;
            List<SellerProductDetailResponseModel>? sellerProductDetails = null;
            List<RelatedProductModel> relatedProducts = new List<RelatedProductModel>();
            List<CategoryResponseModel>? breadcrumb = null;
            int productCategoryId = 0;

            try
            {

                characteristics = await _apiService.GetAsync<List<ProductCharacteristicResponseModel>>($"api/ProductCharacteristics/{firstProductId}");

                shortCharacteristics = await _apiService.GetAsync<List<ProductCharacteristicGroupResponseModel>>($"/api/ProductCharacteristics/short-grouped/{firstProductId}");

                productImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{id}");

                //productResponseModel = await _apiService.GetAsync<ProductResponseModel>($"api/Products/{id}");

                //productCategoryId = productResponseModel.CategoryId;


                categoryDetails = await _apiService.GetAsync<CategoryDetailsResponseModel>($"/api/Categories/getbyproduct/{id}");

                category = await _apiService.GetAsync<CategoryResponseModel>($"api/Categories/{categoryDetails.Id}");

                breadcrumb = await BuildBreadcrumbAsync(category);

                // Отримання пагінованих відгуків
                try
                {
                    const int pageSize = 5; // Кількість відгуків на сторінці
                    feedbacks = await _apiService.GetAsync<FeedbacksPagedResponseModel>($"api/Feedback/paginated/{id}?page={feedbackPage}&pageSize={pageSize}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при отриманні відгуків: {ex.Message}");
                    feedbacks = new FeedbacksPagedResponseModel
                    {
                        Data = new List<FeedbackResponseModel>(),
                        Page = 1,
                        PageSize = 5,
                        TotalItems = 0
                    };
                }

                // Отримання даних продавців
                try
                {
                    sellerProductDetails = await _apiService.GetAsync<List<SellerProductDetailResponseModel>>($"api/SellerProductDetails/{id}");
                    if (sellerProductDetails != null)
                    {
                        // Додаткова перевірка та виправлення порожніх значень
                        foreach (var seller in sellerProductDetails)
                        {
                            if (string.IsNullOrEmpty(seller.StoreName))
                            {
                                seller.StoreName = "Невідомий магазин";
                            }
                            if (string.IsNullOrEmpty(seller.ProductStoreUrl))
                            {
                                seller.ProductStoreUrl = "#";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка отримання SellerProductDetails: {ex.Message}");
                    // Залишаємо порожній список
                }


                var productsWrapper = await _apiService.GetAsync<ProductsResponseWrapper>(
               $"api/Products/bycategorypaginated/{productCategoryId}?page={1}&pageSize={5}");
                               

                // Конвертуємо нову модель у формат, що очікується представленням
                var products = new List<ProductToCategoriesListModel>();

                if (productsWrapper?.Data != null)
                {
                    foreach (var relatedProduct in productsWrapper.Data)
                    {
                        var convertedProduct = new ProductToCategoriesListModel
                        {
                            Id = relatedProduct.ProductGroups.FirstOrDefault()?.FirstProductId ?? relatedProduct.BaseProductId,
                            Title = relatedProduct.Title,
                            Description = relatedProduct.Description,
                            CategoryId = relatedProduct.CategoryId
                        };

                        try
                        {
                            // Все ще потрібно окремо отримувати зображення
                            var relatedProductImages = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{relatedProduct.BaseProductId}");
                            convertedProduct.ImageUrl = productImages?.FirstOrDefault()?.ImageUrl;
                        }
                        catch (Exception ex)
                        {
                            convertedProduct.ImageUrl = null;
                        }
                        products.Add(convertedProduct);
                    }
                }


                var relatedProductsFiltered = products.Where(p => p.Id != id).Take(4).ToList();



                foreach (var relatedProduct in relatedProductsFiltered)
                {
                    // Завантажуємо зображення для кожного пов'язаного товару
                    var imagesForRelated = await _apiService.GetAsync<List<ProductImageModel>>($"api/ProductImage/{relatedProduct.Id}");
                    var imageUrl = imagesForRelated?.FirstOrDefault()?.ImageUrl ?? "placeholder.jpg";

                    relatedProducts.Add(new RelatedProductModel
                    {
                        Id = relatedProduct.Id,
                        ProductName = relatedProduct.Title,
                        ProductPrice = 25000,
                        ImageUrl = imageUrl,
                    });
                }



                var product = new ProductPageModel
                {
                    Characteristics = characteristics ?? new List<ProductCharacteristicResponseModel>(),
                    ShortCharacteristics = shortCharacteristics ?? new List<ProductCharacteristicGroupResponseModel>(),
                    ProductImages = productImages ?? new List<ProductImageModel>(),
                    CategoryBreadcrumb = breadcrumb ?? new List<CategoryResponseModel>(),
                    Feedbacks = feedbacks?.Data ?? new List<FeedbackResponseModel>(),
                    FeedbacksPaging = feedbacks ?? new FeedbacksPagedResponseModel
                    {
                        Data = new List<FeedbackResponseModel>(),
                        Page = 1,
                        PageSize = 5,
                        TotalItems = 0
                    },
                    RelatedProducts = relatedProducts,
                    SellerProductDetails = sellerProductDetails ?? new List<SellerProductDetailResponseModel>(),
                    ProductResponseModel = productResponseModel ?? new ProductResponseModel
                    {
                        Title = "Інформація недоступна",
                        Description = "Опис товару недоступний"
                    },
                    CategoryId = productCategoryId,
                    CurrentFeedbackPage = feedbackPage
                };



                return View(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Загальна помилка: {ex.Message}");
                return View(new ProductPageModel());
            }
        }

        private async Task<List<CategoryResponseModel>> BuildBreadcrumbAsync(CategoryResponseModel category)
        {
            var breadcrumb = new List<CategoryResponseModel>();
            while (category != null)
            {
                breadcrumb.Insert(0, category);

                if (category.ParentCategoryId.HasValue)
                {
                    category = await _apiService.GetAsync<CategoryResponseModel>($"api/Categories/{category.ParentCategoryId.Value}");
                }
                else
                {
                    category = null;
                }
            }
            return breadcrumb;
        }

        //[HttpPost]
        //public async Task<IActionResult> AddFeedback(FeedbackRequestModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["FeedbackError"] = "Будь ласка, перевірте правильність заповнення всіх полів";
        //        return RedirectToAction("Index", new { id = model.ProductId });
        //    }

        //    try
        //    {
        //        // Відправляємо запит на API для додавання відгуку
        //        var response = await _apiService.PostAsync<FeedbackResponseModel>("api/Feedback", model);

        //        TempData["FeedbackSuccess"] = "Ваш відгук успішно додано!";
        //        return RedirectToAction("Index", new { id = model.ProductId });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["FeedbackError"] = "Не вдалося додати відгук. Спробуйте пізніше.";
        //        Console.WriteLine($"Помилка додавання відгуку: {ex.Message}");
        //        return RedirectToAction("Index", new { id = model.ProductId });
        //    }
        //}
    }
}