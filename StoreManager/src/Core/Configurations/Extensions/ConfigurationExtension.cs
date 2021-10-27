using Microsoft.Extensions.Configuration;

namespace Core.Configurations.Extensions
{
    public static class ConfigurationExtension
    {
        public static Setting GetSettings(this IConfiguration configuration)
        {
            var settings = configuration.Get<Setting>();

            return settings;
        }

        public static string GetConnectionsString(this IConfiguration configuration)
        {
            var setting = GetSettings(configuration);

            return setting.DbConnection.PostgresConnection;
        }
        
    }
}