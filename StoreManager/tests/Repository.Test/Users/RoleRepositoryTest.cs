using System;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Users;
using Repository.Test.Configuration;
using Repository.Test.Seeders;
using Xunit;

namespace Repository.Test.Users
{
    public class RoleRepositoryTest : IDisposable
    {
        private readonly RoleRepository _roleRepository;
        private readonly string _databaseName = "rolesDatabase";

        public RoleRepositoryTest()
        {
            var configuration = new RepositoryTestConfiguration().CreateConfigurations(_databaseName);
            DatabaseConfiguration.CreateMigrations(_databaseName);
            _roleRepository = new RoleRepository(configuration, new SqLiteDbConnectionProvider());
        }

        [Fact]
        public async Task GetUserOk()
        {
            var seeder = new RoleSeeder(_roleRepository);
            var count = new Random().Next(1, 10);
            var stubRoles = await seeder.CreateRoles(count);

            for (var i = 0; i < count; i++)
            {
                stubRoles[i].Id = i + 1;
            }

            var roles = await _roleRepository.GetRolesAsync();

            roles.Should().BeEquivalentTo(stubRoles);
        }

        public void Dispose()
        {
            DatabaseConfiguration.RemoveMigrations(_databaseName);
        }
    }
}