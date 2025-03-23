namespace PriceComparisonMVC.Models
{
    public class ProductWithCharacteristicsViewModel
    {
        public Categories.ProductToCategoriesListModel Product { get; set; }
        public List<Response.ProductCharacteristicGroupResponseModel> CharacteristicGroups { get; set; }
    }
}
