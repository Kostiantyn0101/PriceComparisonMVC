using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Models.Categories
{
    public class CategoryListModel
    {
        public CategoryResponseModel ParentCategory { get; set; }

        public List<ProductToCategoriesListModel> ProductToCategories { get; set; }
    }
}
