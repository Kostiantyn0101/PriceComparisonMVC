namespace PriceComparisonMVC.Models.Response
{
    public class CategoryDetailsResponseModel
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string? ImageUrl { get; set; }

        public string? IconUrl { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? ParentCategory { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsParent { get; set; }

        public string? Subcategories { get; set; }

        public string? CategoryCharacteristics { get; set; }

        public string? CategoryCharacteristicGroups { get; set; }

        public string? RelatedCategories { get; set; }

        public string? BaseProducts { get; set; }

        public string? AuctionClickRates { get; set; }
    }
}

