using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Data;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        public async Task<IActionResult> CategoryList(int id)
        {
            try
            {
                var categoryListResult = await _categoryService.GetCategoryListAsync(id);
                ViewBag.CurrentCategory = categoryListResult.CurrentCategory;

                // Отримуємо дані категорій (з того ж джерела, що й для головної сторінки)
                var categories = Data.IndexContentData.GetCategories();

                // Передаємо через ViewBag
                ViewBag.Categories = categories;

                return View(categoryListResult.CategoryListModels);
             }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Сталася помилка при отриманні списку категорій: " + ex.Message;
                return View(new List<CategoryListModel>());
            }
        }

        public async Task<IActionResult> CategoryProductList(int id)
        {
            try
            {
                var result = await _categoryService.GetCategoryProductListAsync(id);
                ViewBag.CurrentCategory = result.CurrentCategory;
                ViewBag.ParentCategory = result.ParentCategory;

                // Отримуємо дані категорій (з того ж джерела, що й для головної сторінки)
                var categories = Data.IndexContentData.GetCategories();

                // Передаємо через ViewBag
                ViewBag.Categories = categories;


                return View(result.ProductsWithCharacteristics);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Сталася помилка при отриманні даних: " + ex.Message;
                return View(new List<ProductWithCharacteristicsViewModel>());
            }
        }
    }
}
