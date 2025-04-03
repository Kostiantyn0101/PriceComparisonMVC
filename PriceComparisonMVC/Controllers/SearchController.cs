using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Services;
using PriceComparisonMVC.Models.Search;
using PriceComparisonMVC.Models.Response;
namespace PriceComparisonMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly IApiService _apiService;

        public SearchController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Search/Results?query={query}
        public async Task<IActionResult> Results(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                //відображення блоку категорій
                var categories = Data.IndexContentData.GetCategories();
                ViewBag.Categories = categories;

                var searchResults = await _apiService.GetAsync<List<ProductSearchResponseModel>>($"api/Products/search/{Uri.EscapeDataString(query)}");

                return View(new SearchResultsViewModel
                {
                    Query = query,
                    Results = searchResults ?? new List<ProductSearchResponseModel>()
                });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Сталася помилка при пошуку: {ex.Message}";
                return View(new SearchResultsViewModel
                {
                    Query = query,
                    Results = new List<ProductSearchResponseModel>()
                });
            }
        }

        // API для автозаповнення пошуку
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                return Json(new List<ProductSearchResponseModel>());
            }

            try
            {
                var searchResults = await _apiService.GetAsync<List<ProductSearchResponseModel>>($"api/Products/search/{Uri.EscapeDataString(query)}");

                return Json(searchResults ?? new List<ProductSearchResponseModel>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка втозаповнення: {ex.Message}");
                return Json(new List<ProductSearchResponseModel>());
            }
        }
    }
}
