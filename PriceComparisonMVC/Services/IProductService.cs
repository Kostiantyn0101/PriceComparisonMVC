using PriceComparisonMVC.Models.Product;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Services
{
    public interface IProductService
    {
        Task<ProductPageModel> GetProductPageModelAsync(int productId, int feedbackPage);
        Task<List<CategoryResponseModel>> BuildBreadcrumbAsync(CategoryResponseModel category);
    }
}
