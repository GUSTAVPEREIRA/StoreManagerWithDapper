using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Core.Users.Interfaces;
using Dummie.Test.Users;
using FluentAssertions;
using Infrastructure.Users;
using NSubstitute.ExceptionExtensions;
using Repository.Test.Configuration;
using Repository.Test.Seeders;
using Xunit;

namespace Repository.Test.Users
{
    public sealed class UserRepositoryTest : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly RoleSeeder _roleSeeder;
        private readonly UserSeeder _userSeeder;
        private const string DatabaseName = "usersDatabase";

        public UserRepositoryTest()
        {
            var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
            DatabaseConfiguration.CreateMigrations(DatabaseName);

            _userRepository = new UserRepository(configuration, new SqLiteDbConnectionProvider());
            var roleRepository = new RoleRepository(configuration, new SqLiteDbConnectionProvider());
            _roleSeeder = new RoleSeeder(roleRepository);
            _userSeeder = new UserSeeder(_userRepository, _roleSeeder);
        }


        [Fact]
        public async Task InsertUserOk()
        {
            var user = new UserDummie().Generate();
            var role = await _roleSeeder.CreateRoles(1);
            user.Role = role.FirstOrDefault();

            var result = await _userRepository.CreateUser(user);
            user.Id = result.Id;

            user.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task UpdateUserOk()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.FirstOrDefault();

            Assert.True(user != null);

            var updatedUser = new UserDummie(user.Role).Generate();
            updatedUser.Id = user.Id;
            await _userRepository.UpdateUser(updatedUser);
            user = await _userRepository.GetUser(user.Id);

            user.Password = updatedUser.Password;
            user.Should().BeEquivalentTo(updatedUser);
        }

        [Fact]
        public async Task GetUserOk()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.FirstOrDefault();
            Assert.True(user != null);

            var result = await _userRepository.GetUser(user.Id);

            user.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersOk()
        {
            var count = new Random().Next(1, 100);
            var users = await _userSeeder.CreateUsers(count);

            var result = await _userRepository.GetUsers();

            users.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersByEmailAndPasswordOk()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.First();

            var result = await _userRepository.GetUserByEmailAndPassword(user.Email, user.Password);
            user.Password = null;
            user.Should().BeEquivalentTo(result);
        }

        public void Dispose()
        {
            DatabaseConfiguration.RemoveMigrations(DatabaseName);
        }
    }
}