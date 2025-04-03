using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PriceComparisonMVC.Filter
{
    public class UserInfoActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Виконується перед викликом дії контролера
            if (context.Controller is Controller controller)
            {
                // Отримуємо ім'я користувача з контексту
                string username = context.HttpContext.User?.Identity?.Name;

                // Додаємо ім'я користувача до ViewBag
                controller.ViewBag.Username = username;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Виконується після виклику дії контролера (тут нічого не робимо)
        }
    }
}
