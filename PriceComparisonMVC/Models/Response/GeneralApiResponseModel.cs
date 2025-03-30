namespace PriceComparisonMVC.Models.Response
{
    public class GeneralApiResponseModel
    {
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
