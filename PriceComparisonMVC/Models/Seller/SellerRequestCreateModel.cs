using System.ComponentModel.DataAnnotations;

namespace PriceComparisonMVC.Models.Seller
{
    public class SellerRequestCreateModel
    {
        public int UserId { get; set; }
        public string StoreName { get; set; }
        public string WebsiteUrl { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string? StoreComment { get; set; }
    }
}
