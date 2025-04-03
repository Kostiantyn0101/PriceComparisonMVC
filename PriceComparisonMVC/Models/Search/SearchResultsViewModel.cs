using PriceComparisonMVC.Models.Response;

namespace PriceComparisonMVC.Models.Search
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<ProductSearchResponseModel> Results { get; set; }
    }
}
