using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Product;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;

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
    }
}