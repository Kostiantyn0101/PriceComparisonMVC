namespace PriceComparisonMVC.Models.Response
{
    public class BaseProductResponseModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsUnderModeration { get; set; }
        public DateTime AddedToDatabase { get; set; }
        public int CategoryId { get; set; }
    }
}
