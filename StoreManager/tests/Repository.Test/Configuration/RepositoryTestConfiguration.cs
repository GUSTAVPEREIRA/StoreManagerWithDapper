using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Repository.Test.Configuration
{
    public class RepositoryTestConfiguration
    {
        private readonly Dictionary<string, string> _configurations;

        public RepositoryTestConfiguration()
        {
            _configurations = new Dictionary<string, string>();
        }

        public IConfigurationRoot CreateConfigurations(string database)
        {
            _configurations.Add("ConnectionStrings:StoreManagerDB", DatabaseConfiguration.GetConnectionString(database));
            
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(_configurations)
                .Build();

            return configuration;
        }

        public void AdConfiguration(string key, string configuration)
        {
            _configurations.Add(key, configuration);
        }
    }
}