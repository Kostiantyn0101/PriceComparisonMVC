namespace PriceComparisonMVC.Models.Response
{
    public class ProductCharacteristicResponseModel
    {
        public int? ProductId { get; set; }
        public int? BaseProductId { get; set; }
        public int CharacteristicId { get; set; }
        public string CharacteristicTitle { get; set; }
        public string? CharacteristicUnit { get; set; }
        public int DisplayOrder { get; set; }
        public string CharacteristicDataType { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public bool? ValueBoolean { get; set; }
        public DateTime? ValueDate { get; set; }
    }
}
