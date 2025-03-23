namespace PriceComparisonMVC.Models.Response
{
    public class CategoryResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        public CategoryResponseModel? ParentCategory { get; set; }
        public ICollection<CategoryResponseModel>? Subcategories { get; set; }
        public int DisplayOrder { get; set; }

    }
}
