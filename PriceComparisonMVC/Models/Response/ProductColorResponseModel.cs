namespace PriceComparisonMVC.Models.Response
{
    public class ProductColorResponseModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HexCode { get; set; } = string.Empty;
        public string? GradientCode { get; set; }
    }
}
