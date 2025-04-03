using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TokenManager _tokenManager;
        private readonly IApiService _apiService;

        public HomeController(ILogger<HomeController> logger, TokenManager tokenManager, IApiService apiService)
        {
            _logger = logger;
            _tokenManager = tokenManager;
            _apiService = apiService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var indexContent = Data.IndexContentData.GetIndexContent();

            return View(indexContent);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
