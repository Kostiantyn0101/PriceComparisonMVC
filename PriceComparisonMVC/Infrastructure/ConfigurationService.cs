using PriceComparisonMVC.Infrastructure.DependencyInjection;
using PriceComparisonWebAPI.Infrastructure.DependencyInjection;

namespace PriceComparisonMVC.Infrastructure
{
    public class ConfigurationService
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            try
            {
                builder.AddConfiguration();
                builder.AddFluentValidation();
                builder.AddRazorRuntimeCompilation();
                builder.AddServices();
                builder.AddAuth();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
