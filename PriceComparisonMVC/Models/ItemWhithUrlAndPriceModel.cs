namespace PriceComparisonMVC.Models
{
    public class ItemWhithUrlAndPriceModel
    {
        public string ProductDescription { get; set; }
        public string ProductPrice { get; set; }
        public string IconUrl { get; set; }
        public string? AvatarUrl { get; set; }

        public string ProductId { get; set; }
        public string Category { get; set; }
       
    }
}
