namespace PriceComparisonMVC.Models.Compare
{
    public class ComparisonModels
    {
        // Класи для десеріалізації відповіді API
        public class ComparisonApiResponse
        {
            public string ProductATitle { get; set; }
            public string ProductBTitle { get; set; }
            public List<CharacteristicGroup> ProductA { get; set; }
            public List<CharacteristicGroup> ProductB { get; set; }
            public string Explanation { get; set; }
            public string AiProvider { get; set; }
        }

        public class CharacteristicGroup
        {
            public string CharacteristicGroupTitle { get; set; }
            public List<ProductCharacteristic> ProductCharacteristics { get; set; }
        }

        public class ProductCharacteristic
        {
            public string CharacteristicTitle { get; set; }
            public string Value { get; set; }
            public bool IsHighlighted { get; set; }
        }
    }
}
