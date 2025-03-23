using System.Collections.Generic;

namespace PriceComparisonMVC.Models.Response
{
    public class PaginatedResponseModel<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
    }
}