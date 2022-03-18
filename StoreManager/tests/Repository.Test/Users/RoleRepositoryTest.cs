using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
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
    public sealed class RoleRepositoryTest : IDisposable
    {
        private readonly RoleRepository _roleRepository;
        private const string DatabaseName = "rolesDatabase";
        private readonly RoleSeeder _seeder;
        private readonly IMapper _mapper;

        public RoleRepositoryTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(RoleMappingProfile));
                cfg.CreateMap<RoleResponse, RoleUpdatedRequest>().ReverseMap();
            });

            _mapper = mapperConfiguration.CreateMapper();

            var configuration = new RepositoryTestConfiguration().CreateConfigurations(DatabaseName);
            DatabaseConfiguration.CreateMigrations(DatabaseName);
            _roleRepository = new RoleRepository(configuration, new SqLiteDbConnectionProvider(), _mapper);
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
            var roleRequest = new RoleRequestDummie().Generate();

            var result = await _roleRepository.CreateRoleAsync(roleRequest);

            result.Id.Should().BeGreaterThan(0);
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
            var expectationResult = await _seeder.CreateRoles(count);
            var roleUpdatedRequests = _mapper.Map<List<RoleResponse>, List<RoleUpdatedRequest>>(expectationResult);

            var faker = new Faker();

            foreach (var roleUpdatedRequest in roleUpdatedRequests)
            {
                roleUpdatedRequest.Name = faker.Person.FullName;
                roleUpdatedRequest.IsAdmin = faker.Random.Bool();

                await _roleRepository.UpdateRoleAsync(roleUpdatedRequest);
            }

            expectationResult = _mapper.Map<List<RoleUpdatedRequest>, List<RoleResponse>>(roleUpdatedRequests);
            var roleResponsesResult = await _roleRepository.GetRolesAsync();
            
            roleResponsesResult = roleResponsesResult.ToList();
            roleResponsesResult.Count().Should().Be(expectationResult.Count);
            roleResponsesResult.Should().BeEquivalentTo(expectationResult);
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