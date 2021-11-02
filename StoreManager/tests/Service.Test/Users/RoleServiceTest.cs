using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Users;
using AutoMapper;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Mappings;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Service.Test.Users
{
    public class RoleServiceTest
    {
        private readonly IRoleService _roleService;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleServiceTest()
        {
            var mockMapper = new MapperConfiguration(x => { x.AddProfile(new RoleMappingProfile()); });

            _mapper = mockMapper.CreateMapper();

            _roleRepository = Substitute.For<IRoleRepository>();
            _roleService = new RoleService(_mapper, _roleRepository);
        }

        [Fact]
        public async Task CreatedRoleOk()
        {
            var role = new RoleDummie().Generate();
            var resultExpected = _mapper.Map<RoleResponse>(role);
            var roleRequest = _mapper.Map<RoleRequest>(role);

            _roleRepository.CreateRoleAsync(Arg.Any<Role>()).Returns(role);
            var result = await _roleService.CreateRoleAsync(roleRequest);

            result.Should().BeEquivalentTo(resultExpected);
            await _roleRepository.Received().CreateRoleAsync(Arg.Any<Role>());
        }

        [Fact]
        public async Task UpdatedRoleOk()
        {
            var role = new RoleDummie().Generate();
            role.Id = 1;
            var resultExpected = _mapper.Map<RoleResponse>(role);
            var roleUpdatedRequest = _mapper.Map<RoleUpdatedRequest>(role);

            _roleRepository.UpdateRoleAsync(Arg.Any<Role>()).Returns(role);

            var result = await _roleService.UpdateRoleAsync(roleUpdatedRequest);

            result.Should().BeEquivalentTo(resultExpected);
            await _roleRepository.Received().UpdateRoleAsync(Arg.Any<Role>());
        }

        [Fact]
        public async Task GetRole()
        {
            var role = new RoleDummie().Generate();
            role.Id = new Random().Next(1, 9999);

            var resultExpected = _mapper.Map<RoleResponse>(role);
            _roleRepository.GetRoleAsync(Arg.Any<int>()).Returns(role);

            var result = await _roleService.GetRoleAsync(role.Id);

            result.Should().BeEquivalentTo(resultExpected);
            await _roleRepository.Received().GetRoleAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task GetRoles()
        {
            var roles = new RoleDummie().Generate(new Random().Next(1, 100));
            var resultExpected = _mapper.Map<List<RoleResponse>>(roles);
            _roleRepository.GetRolesAsync().Returns(roles);

            var result = await _roleService.GetRolesAsync();

            result.Should().BeEquivalentTo(resultExpected);
            await _roleRepository.Received().GetRolesAsync();
        }

        [Fact]
        public async Task DeleteRole()
        {
            _roleRepository.DeleteRoleAsync(Arg.Any<int>()).Returns(Task.FromResult);

            await _roleService.DeleteRoleAsync(new Random().Next(1, 100));

            await _roleRepository.Received().DeleteRoleAsync(Arg.Any<int>());
        }
    }
}