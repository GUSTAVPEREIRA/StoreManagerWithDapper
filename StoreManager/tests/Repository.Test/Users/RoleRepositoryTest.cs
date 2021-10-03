using System.Threading.Tasks;
using AutoMapper.Configuration;
using Infrastructure.Users;
using Repository.Test.Configuration;
using Xunit;

namespace Repository.Test.Users
{
    
    public class RoleRepositoryTest
    {
        private readonly RoleRepository _roleRepository;
        private readonly string _databaseName = "roles";

        public RoleRepositoryTest()
        {
            var configuration = new RepositoryTestConfiguration().CreateConfigurations(_databaseName);
            DatabaseConfiguration.CreateMigrations(_databaseName);
            _roleRepository = new RoleRepository(configuration);
        }

        [Fact]
        public async Task GetUser()
        {
            
        }
    }
}