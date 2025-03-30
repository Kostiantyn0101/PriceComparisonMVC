using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Models.Seller;
using PriceComparisonMVC.Services;

namespace PriceComparisonMVC.Controllers
{
    [Controller]
    public class SellerRequestController : Controller
    {
        private readonly IApiService _apiService;

        public SellerRequestController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return View("LoginNeeded");
                }

                var model = new SellerRequestCreateModel
                {
                    UserId = int.Parse(userIdClaim.Value)
                };

                return View(model);
            }
            else
            {
                return View("LoginNeeded");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSellerRequest(SellerRequestCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var result = await _apiService.PostAsync<SellerRequestCreateModel, GeneralApiResponseModel>("/api/SellerRequest/create", model);
                ViewData["Message"] = "Заявка успішно відправлена. Чекайте на відповідь.";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Сталася помилка. Спробуйте пізніше";
            }

            return View();
        }
    }
}
