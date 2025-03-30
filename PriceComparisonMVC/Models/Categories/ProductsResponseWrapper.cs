namespace PriceComparisonMVC.Models.Categories
{
    public class ProductsResponseWrapper
    {
        public List<ShortProductResponseModel> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public class ShortProductResponseModel
    {
        public int BaseProductId { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public List<ShortProductGroup> ProductGroups { get; set; }
        public string ImageUrl { get; set; } // Додано для сумісності з існуючим кодом
    }

    //public class ShortProductGroup
    //{
    //    public int Id { get; set; }
    //    public int FirstProductId { get; set; }
    //    public string Name { get; set; }
    //    public int DisplayOrder { get; set; }
    //}


}
