using System;
using System.Threading.Tasks;
using Api.Users;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
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
            var roleResponse = new RoleResponseDummie().Generate();
            _roleService.UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>()).Returns(roleResponse);

            var result = (ObjectResult) await _roleController.UpdateRole(roleRequest);

            result.Value.Should().BeEquivalentTo(roleResponse);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            await _roleService.Received().UpdateRoleAsync(Arg.Any<RoleUpdatedRequest>());
        }

        [Fact]
        public async Task GetRoleOk()
        {
            var roleResponse = new RoleResponseDummie().Generate();
            _roleService.GetRoleAsync(Arg.Any<int>()).Returns(roleResponse);

            var result = (ObjectResult) await _roleController.GetRole(roleResponse.Id);

            await _roleService.Received().GetRoleAsync(Arg.Any<int>());
            result.Value.Should().BeEquivalentTo(roleResponse);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Fact]
        public async Task GetRolesOk()
        {
            var roleResponse = new RoleResponseDummie().Generate(new Random().Next(1, 100));
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

        [Fact]
        public void ValidationRoleRequestWithoutError()
        {
            var roleRequest = new RoleRequestDummie().Generate();
            var result = new RoleRequestValidation().TestValidate(roleRequest);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
        
        [Fact]
        public void ValidationRoleRequestWithError()
        {
            var roleRequest = new RoleRequestDummie().Generate();
            roleRequest.Name = "";
            var result = new RoleRequestValidation().TestValidate(roleRequest);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void ValidationRoleUpdatedRequestWithoutError()
        {
            var roleUpdatedRequest = new RoleRequestUpdatedDummie().Generate();
            var result = new RoleUpdatedRequestValidation().TestValidate(roleUpdatedRequest);
            
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
        
        [Fact]
        public void ValidationRoleUpdatedRequestWithError()
        {
            var roleUpdatedRequest = new RoleUpdatedRequest();
            var result = new RoleUpdatedRequestValidation().TestValidate(roleUpdatedRequest);
            
            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
    }
}