namespace PriceComparisonMVC.Models.Product
{
    public class RelatedProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal MinPrice { get; set; } = decimal.Zero;
        public decimal MaxPrice { get; set; } = decimal.Zero;
        public string ImageUrl { get; set; } = string.Empty;
        public bool HasPrices => MinPrice > 0 || MaxPrice > 0;
    }
}