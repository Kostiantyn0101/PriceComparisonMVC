namespace PriceComparisonMVC.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData);
        Task PostAsync<TRequest>(string endpoint, TRequest requestData);
    }
}
