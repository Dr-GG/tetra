using Microsoft.Extensions.Configuration;

namespace Tetra.Application.Console.Extensions
{
    public static class ConfigurationExtensions
    {
        public static TSettings BindSettings<TSettings>(
            this IConfiguration configuration, 
            string key) where TSettings : class, new()
        {
            var result = new TSettings();

            configuration.Bind(key, result);

            return result;
        }
    }
}
