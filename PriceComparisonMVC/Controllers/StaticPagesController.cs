using Microsoft.AspNetCore.Mvc;

namespace PriceComparisonMVC.Controllers
{
    public class StaticPagesController : Controller
    {
        // Про нас
        public IActionResult About()
        {
            return View();
        }

        //public IActionResult HowWeWork()
        //{
        //    return View();
        //}

        //public IActionResult Partners()
        //{
        //    return View();
        //}

        // Корисна інформація
        public IActionResult UsefulInfo()
        {
            return View();
        }

        //public IActionResult StoreReviews()
        //{
        //    return View();
        //}

        //public IActionResult HowToChooseProduct()
        //{
        //    return View();
        //}

        //public IActionResult FAQ()
        //{
        //    return View();
        //}

        // Правова інформація
        public IActionResult TermsOfUse()
        {
            return View();
        }

        //public IActionResult PrivacyPolicy()
        //{
        //    return View();
        //}

        // Контакти
        public IActionResult ContactUs()
        {
            return View();
        }

        //public IActionResult SiteMap()
        //{
        //    return View();
        //}
    }
}