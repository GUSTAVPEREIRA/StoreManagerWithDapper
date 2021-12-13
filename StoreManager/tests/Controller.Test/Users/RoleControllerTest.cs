using System;
using System.Threading.Tasks;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Users;
using Xunit;

namespace Controller.Test.Users
{
    public class RoleControllerTest
    {
        private readonly IRoleService _roleService;
        private readonly RoleController _roleController;

        public RoleControllerTest()
        {
            _roleService = Substitute.For<IRoleService>();

            _roleController = new RoleController(_roleService);
        }

        [Fact]
        public async Task CreateRoleOk()
        {
            var roleRequest = new RoleRequestDummie().Generate();
            var expectedResult = new RoleResponse
            {
                Id = 1,
                Name = roleRequest.Name,
                IsAdmin = roleRequest.IsAdmin
            };

            _roleService.CreateRoleAsync(Arg.Any<RoleRequest>()).Returns(expectedResult);

            var result = (ObjectResult) await _roleController.CreateRole(roleRequest);

            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            await _roleService.Received().CreateRoleAsync(Arg.Any<RoleRequest>());
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task UpdatedRoleOk()
        {
            var roleRequest = new RoleRequestUpdatedDummie().Generate();
            var roleResponse = new RoleReponseDummie().Generate();
            _roleService.UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>()).Returns(roleResponse);

            var result = (ObjectResult) await _roleController.UpdateRole(roleRequest);

            result.Value.Should().BeEquivalentTo(roleResponse);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            await _roleService.Received().UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>());
        }

        [Fact]
        public async Task GetRoleOk()
        {
            var roleResponse = new RoleReponseDummie().Generate();
            _roleService.GetRoleAsync(Arg.Any<int>()).Returns(roleResponse);

            var result = (ObjectResult) await _roleController.GetRole(roleResponse.Id);

            await _roleService.Received().GetRoleAsync(Arg.Any<int>());
            result.Value.Should().BeEquivalentTo(roleResponse);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Fact]
        public async Task GetRolesOk()
        {
            var roleResponse = new RoleReponseDummie().Generate(new Random().Next(1, 100));
            _roleService.GetRolesAsync().Returns(roleResponse);

            var result = (ObjectResult) await _roleController.ListRoles();

            await _roleService.Received().GetRolesAsync();
            result.Value.Should().BeEquivalentTo(roleResponse);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task DeleteRoleOk()
        {
            _roleService.DeleteRoleAsync(Arg.Any<int>()).Returns(Task.FromResult);

            var result = (StatusCodeResult) await _roleController.DeleteRole(new Random().Next(1, 9999));

            await _roleService.Received().DeleteRoleAsync(Arg.Any<int>());
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
    }
}