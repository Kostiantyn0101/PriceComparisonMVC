using PriceComparisonMVC.Models.Categories;
using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Models.User
{
    public class UserProfileModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int ReviewsCount { get; set; }

        // Додавання властивості для зберігання переглянутих товарів
        public List<CategoryWithProducts> RecentlyViewedCategoryGroups { get; set; }

        // Додавання властивості для зберігання обраних товарів
        public List<CategoryWithProducts> FavoriteCategoryGroups { get; set; }

        // Додавання властивості для зберігання товарів для порівняння
        public List<CategoryWithProducts> ComparisonCategoryGroups { get; set; }

        public UserProfileModel()
        {
            RecentlyViewedCategoryGroups = new List<CategoryWithProducts>();
            FavoriteCategoryGroups = new List<CategoryWithProducts>();
            ComparisonCategoryGroups = new List<CategoryWithProducts>();
        }
    }

    // Універсальний клас для групування товарів за категоріями
    public class CategoryWithProducts
    {
        public CategoryResponseModel Category { get; set; }
        public List<ProductToCategoriesListModel> Products { get; set; }

        public CategoryWithProducts()
        {
            Products = new List<ProductToCategoriesListModel>();
        }
    }
}