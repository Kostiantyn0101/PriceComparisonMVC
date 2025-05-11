using PriceComparisonMVC.Models.Configuration;

namespace PriceComparisonMVC.Infrastructure.DependencyInjection
{
    public static class AddConfigurationExtension
    {
        public static void AddConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);
            
            builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.Position));
        }
    }
}
