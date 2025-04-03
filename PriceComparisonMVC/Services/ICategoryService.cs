using PriceComparisonMVC.Models;
using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Services
{
    public interface ICategoryService
    {
        Task<CategoryListResult> GetCategoryListAsync(int categoryId);
        Task<CategoryProductListResult> GetCategoryProductListAsync(int categoryId);
        Task<List<ProductToCategoriesListModel>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
        Task<CategoryResponseModel> GetCategoryInfoAsync(int categoryId);
    }

    public class CategoryListResult
    {
        public CategoryResponseModel? CurrentCategory { get; set; }
        public List<CategoryListModel> CategoryListModels { get; set; } = new List<CategoryListModel>();
    }

    public class CategoryProductListResult
    {
        public CategoryResponseModel? CurrentCategory { get; set; }
        public CategoryResponseModel? ParentCategory { get; set; }
        public List<ProductWithCharacteristicsViewModel> ProductsWithCharacteristics { get; set; } = new List<ProductWithCharacteristicsViewModel>();
    }
}