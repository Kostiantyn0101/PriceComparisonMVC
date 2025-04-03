namespace PriceComparisonMVC.Models.Compare
{
    public class SmartComparisonViewModel
    {
        public string ProductAName { get; set; }
        public string ProductAImageUrl { get; set; }
        public string ProductBName { get; set; }
        public string ProductBImageUrl { get; set; }
        public string Explanation { get; set; }
        public List<KeyDifference> KeyDifferences { get; set; } = new List<KeyDifference>();
    }

    public class KeyDifference
    {
        public string CharacteristicName { get; set; }
        public string ProductAValue { get; set; }
        public string ProductBValue { get; set; }
        public string Winner { get; set; }
    }
}



