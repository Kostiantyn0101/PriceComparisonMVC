namespace PriceComparisonMVC.Models
{
    public class IndexContentModel
    {
        public List<CategoryModel> Categories { get; set; }
        public List<ItemToViewModel> PopulaCategoriesImages { get; set; }
        public List<ItemWhithUrlAndPriceModel> PopularProducts { get; set; }
        //public List<String> PopularCategory {  get; set; }

        public List<PopularCategoryModel> PopularCategories { get; set; }
        public List<String> ActualCategory { get; set; }
        public List<CategoryModel> ActualCategories { get; set; }
        public List<ItemWhithUrlAndPriceModel> RecommendedVideos { get; set; }
        public List<ReviewModel> ReviewModels { get; set; }
        public string SelectedCategory { get; set; }
        public Dictionary<string, List<ItemWhithUrlAndPriceModel>> ProductsByCategory { get; set; }

    }
}
