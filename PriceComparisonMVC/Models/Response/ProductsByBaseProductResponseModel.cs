namespace PriceComparisonMVC.Models.Response
{
    public class ProductsByBaseProductResponseModel
    {
        public int Id { get; set; }
        public int BaseProductId { get; set; }
        public ProductGroupModel ProductGroup { get; set; }
        public string ModelNumber { get; set; }
        public string Gtin { get; set; }
        public string Upc { get; set; }
        public int ColorId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsUnderModeration { get; set; }
        public DateTime AddedToDatabase { get; set; }
    }

    public class ProductGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductGroupTypeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
