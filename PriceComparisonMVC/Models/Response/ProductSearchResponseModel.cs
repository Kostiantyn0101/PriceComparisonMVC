namespace PriceComparisonMVC.Models.Response
{
    public class ProductSearchResponseModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
