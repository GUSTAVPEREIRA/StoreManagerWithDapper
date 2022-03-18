using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Infrastructure.Users;
using Infrastructure.Users.Mappings;
using Repository.Test.Configuration;
using Repository.Test.Seeders;
using Xunit;

namespace Repository.Test.Users
{
    public sealed class UserRepositoryTest : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly RoleSeeder _roleSeeder;
        private readonly UserSeeder _userSeeder;
        private readonly IMapper _mapper;
        private const string DatabaseName = "usersDatabase";

        public UserRepositoryTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(RoleMappingProfile));
                cfg.AddMaps(typeof(UserMappingProfile));
                cfg.CreateMap<UserUpdatedRequest, UserResponse>().BeforeMap((userUpdatedRequest, userResponse) =>
                {
                    if (userUpdatedRequest.RoleId != 0)
                    {
                        userResponse.Role = new RoleResponse
                        {
                            Id = userUpdatedRequest.RoleId
                        };
                    }
                });
                
                cfg.CreateMap<RoleResponse, RoleUpdatedRequest>();
            });

            var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
            DatabaseConfiguration.CreateMigrations(DatabaseName);

            _mapper = mapperConfiguration.CreateMapper();
            _userRepository = new UserRepository(configuration, new SqLiteDbConnectionProvider(), _mapper);
            _roleRepository = new RoleRepository(configuration, new SqLiteDbConnectionProvider(), _mapper);
            _roleSeeder = new RoleSeeder(_roleRepository);
            _userSeeder = new UserSeeder(_userRepository, _roleSeeder);
        }


        [Fact]
        public async Task InsertUserOk()
        {
            var userRequest = new UserRequestDummie().Generate();
            var roleResponses = await _roleSeeder.CreateRoles(1);
            var roleResponse = roleResponses.FirstOrDefault();
            userRequest.RoleId = roleResponse!.Id;

            var result = await _userRepository.CreateUserAsync(userRequest);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateUserOk()
        {
            var userResponses = await _userSeeder.CreateUsers(1);
            var userResponse = userResponses.First();

            var updatedUser = new UserUpdateRequestDummie().Generate();
            updatedUser.RoleId = userResponse.Role.Id;
            updatedUser.Id = userResponse.Id;

            await _userRepository.UpdateUserAsync(updatedUser);
            userResponse = await _userRepository.GetUserAsync(userResponse.Id);

            var expectedValue = _mapper.Map<UserUpdatedRequest, UserResponse>(updatedUser);
            expectedValue.Role = await _roleRepository.GetRoleAsync(expectedValue.Role.Id);
            userResponse.Password = expectedValue.Password;
            userResponse.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task GetUserOk()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.First();
            user.Role = await _roleRepository.GetRoleAsync(user.Role.Id);

            var result = await _userRepository.GetUserAsync(user.Id);

            user.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersOk()
        {
            var count = new Random().Next(1, 100);
            var userResponses = await _userSeeder.CreateUsers(count);

            foreach (var userResponse in userResponses)
            {
                userResponse.Role = await _roleRepository.GetRoleAsync(userResponse.Role.Id);
            }

            var result = await _userRepository.GetUsersAsync();

            userResponses.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersByEmailAndPasswordOk()
        {
            var users = await _userSeeder.CreateUsers(1);
            var userResponse = users.First();
            var roleResponse = await _roleRepository.GetRoleAsync(userResponse.Role.Id);
            userResponse.Role = roleResponse;


            var result =
                await _userRepository.GetUserByEmailAndPasswordAsync(userResponse.Email, userResponse.Password);

            result.Role = roleResponse;
            userResponse.Password = null;
            userResponse.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersByEmail()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.First();
            user.Role = await _roleRepository.GetRoleAsync(user.Role.Id);

            var result = await _userRepository.GetUserByEmailAsync(user.Email);

            user.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetUsersByEmailNotFound()
        {
            var users = await _userSeeder.CreateUsers(1);
            var user = users.First();

            var result = await _userRepository.GetUserByEmailAsync(user.Email + "asd56as54d");

            result.Should().BeNull();
        }


        [Fact]
        public async Task ChangePasswordOk()
        {
            var users = await _userSeeder.CreateUsers(new Random().Next(2, 5));
            var user = users.First();
            var userUpdatedRequest = new UserUpdatedRequest
            {
                Password = user.Password,
                Id = user.Id
            };

            var result = await _userRepository.ChangeUserPasswordAsync(userUpdatedRequest);

            result.Password.Should().BeEquivalentTo(userUpdatedRequest.Password);
        }

        public void Dispose()
        {
            DatabaseConfiguration.RemoveMigrations(DatabaseName);
        }
    }
}