using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Dummie.Test.Users;
using FluentAssertions;
using Infrastructure.Users;
using Repository.Test.Configuration;
using Repository.Test.Seeders;
using Xunit;

namespace Repository.Test.Users
{
    public sealed class RoleRepositoryTest : IDisposable
    {
        private readonly RoleRepository _roleRepository;
        private const string DatabaseName = "rolesDatabase";
        private readonly RoleSeeder _seeder;

        public RoleRepositoryTest()
        {
            var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
            DatabaseConfiguration.CreateMigrations(DatabaseName);
            _roleRepository = new RoleRepository(configuration, new SqLiteDbConnectionProvider());
            _seeder = new RoleSeeder(_roleRepository);
        }

        [Fact]
        public async Task GetRolesOk()
        {
            var count = new Random().Next(1, 10);
            var roles = await _seeder.CreateRoles(count);

            var result = await _roleRepository.GetRolesAsync();

            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task GetRolesNotFound()
        {
            var roles = await _roleRepository.GetRolesAsync();
            roles.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task InsertRoleOk()
        {
            var newRole = new RoleDummie().Generate();

            var result = await _roleRepository.CreateRoleAsync(newRole);
            newRole.Id = result.Id;

            result.Should().BeEquivalentTo(newRole);
        }

        [Fact]
        public async Task DeleteRoleOk()
        {
            var roles = await _seeder.CreateRoles(1);
            var role = roles.First();

            await _roleRepository.DeleteRoleAsync(role.Id);
            var result = await _roleRepository.GetRolesAsync();

            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateRoleOk()
        {
            var count = new Random().Next(1, 10);
            var roles = await _seeder.CreateRoles(count);

            var faker = new Faker();

            foreach (var role in roles)
            {
                role.Name = faker.Person.FullName;
                role.IsAdmin = faker.Random.Bool();

                await _roleRepository.UpdateRoleAsync(role);
            }

            var results = await _roleRepository.GetRolesAsync();

            results.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task GetRoleAsyncOk()
        {
            var count = new Random().Next(1, 10);
            var roles = await _seeder.CreateRoles(count);
            var role = roles.First(x => x.Id == count);

            var result = await _roleRepository.GetRoleAsync(role.Id);

            result.Should().BeEquivalentTo(role);
        }
        
        [Fact]
        public async Task GetRoleAsyncNotFound()
        {
            var result = await _roleRepository.GetRoleAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CheckIfRoleExisting()
        {
            var count = new Random().Next(1, 10);
            var roles = await _seeder.CreateRoles(count);
            var role = roles.First(x => x.Id == count);
            
            var result = await _roleRepository.CheckIfRoleExist(role.Id);

            result.Should().Be(true);
        }
        
        [Fact]
        public async Task CheckIfRoleIsntExisting()
        {
            var count = new Random().Next(1, 10);
            var roles = await _seeder.CreateRoles(count);
            var role = roles.First(x => x.Id == count);
            
            var result = await _roleRepository.CheckIfRoleExist(role.Id + 1);

            result.Should().Be(false);
        }
        
        public void Dispose()
        {
            DatabaseConfiguration.RemoveMigrations(DatabaseName);
        }
    }
}