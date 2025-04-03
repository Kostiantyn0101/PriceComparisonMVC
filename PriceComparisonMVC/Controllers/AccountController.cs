using Microsoft.AspNetCore.Mvc;
using PriceComparisonMVC.Models.Response;
using PriceComparisonMVC.Models.User;
using PriceComparisonMVC.Services;
using PriceComparisonMVC.Models.Request;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PriceComparisonMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly TokenManager _tokenManager;

        public AccountController(IAuthService authService, TokenManager tokenManager)
        {
            _authService = authService;
            _tokenManager = tokenManager;
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





        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
               
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

    }
}
