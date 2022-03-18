using System;
using System.Threading.Tasks;
using Application.Users;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Service.Test.Users;

public class RoleServiceTest
{
    private readonly IRoleService _roleService;
    private readonly IRoleRepository _roleRepository;

    public RoleServiceTest()
    {
        _roleRepository = Substitute.For<IRoleRepository>();
        _roleService = new RoleService(_roleRepository);
    }

    [Fact]
    public async Task CreatedRoleOk()
    {
        var roleResponse = new RoleResponseDummie().Generate();
        var roleRequest = new RoleRequest
        {
            Name = roleResponse.Name,
            IsAdmin = roleResponse.IsAdmin
        };

        _roleRepository.CreateRoleAsync(Arg.Any<RoleRequest>()).Returns(roleResponse);
        var result = await _roleService.CreateRoleAsync(roleRequest);

        roleResponse.Id = result.Id;

        result.Should().BeEquivalentTo(roleResponse);
        await _roleRepository.Received().CreateRoleAsync(Arg.Any<RoleRequest>());
    }

    [Fact]
    public async Task UpdatedRoleOk()
    {
        var roleUpdatedRequest = new RoleRequestUpdatedDummie().Generate();

        var roleResponse = new RoleResponse()
        {
            Id = roleUpdatedRequest.Id,
            Name = roleUpdatedRequest.Name,
            IsAdmin = roleUpdatedRequest.IsAdmin,
        };

        _roleRepository.UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>()).Returns(roleResponse);

        var result = await _roleService.UpdateRoleAsync(roleUpdatedRequest);
        result.Should().BeEquivalentTo(roleUpdatedRequest);
        await _roleRepository.Received().UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>());
    }

    [Fact]
    public async Task GetRole()
    {
        var roleResponse = new RoleResponseDummie().Generate();
        roleResponse.Id = new Random().Next(1, 9999);

        _roleRepository.GetRoleAsync(Arg.Any<int>()).Returns(roleResponse);

        var result = await _roleService.GetRoleAsync(roleResponse.Id);

        result.Should().BeEquivalentTo(roleResponse);
        await _roleRepository.Received().GetRoleAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetRoles()
    {
        var roleResponses = new RoleResponseDummie().Generate(new Random().Next(1, 100));
        _roleRepository.GetRolesAsync().Returns(roleResponses);

        var result = await _roleService.GetRolesAsync();

        result.Should().BeEquivalentTo(roleResponses);
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