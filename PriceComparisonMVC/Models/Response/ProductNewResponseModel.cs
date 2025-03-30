namespace PriceComparisonMVC.Models.Response
{
    
    public class ProductNewResponseModel
    {
        public int Id { get; set; }
        public int BaseProductId { get; set; }
        public ProductGroupResponseModel? ProductGroup { get; set; }
        public string ModelNumber { get; set; } = string.Empty;
        public string Gtin { get; set; } = string.Empty;
        public string? Upc { get; set; }
        public int ColorId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsUnderModeration { get; set; }
        public DateTime AddedToDatabase { get; set; }

        // Додаткові властивості, які вам можуть знадобитися в коді
        //public string Title => $"Модель {ModelNumber}";

        private string _title;
        public string Title
        {
            get { return _title ?? $"Модель {ModelNumber}"; }
            set { _title = value; }
        }


        public string Brand { get; set; } = string.Empty; // Цю властивість, можливо, потрібно заповнювати окремо
    }

    public class ProductGroupResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductGroupTypeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
