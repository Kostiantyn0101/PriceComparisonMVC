
namespace PriceComparisonMVC.Models.Compare
{
    public class ProductComparisonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int OfferCount { get; set; }
        public string ImageUrl { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public bool HasOffers { get; set; }
        public int CategoryId { get; set; }
    }
}
