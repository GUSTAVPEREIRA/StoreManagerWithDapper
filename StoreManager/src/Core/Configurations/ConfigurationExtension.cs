using Microsoft.Extensions.Configuration;

namespace Core.Configurations
{
    public static class ConfigurationExtension
    {
        public static Setting GetSettings(this IConfiguration configuration)
        {
            var settings = configuration.Get<Setting>();

            return settings;
        }

        public static string GetConnectionString(this IConfiguration configuration)
        {
            var setting = GetSettings(configuration);

            return setting.DbConnection.PostgresConnection;
        }
        
    }
}