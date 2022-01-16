using System.Threading.Tasks;
using Api.Users;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Controller.Test.Users
{
    public class UserControllerTest
    {
        private readonly IUserService _userService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _userService = Substitute.For<IUserService>();
            _userController = new UserController(_userService);
        }

        [Fact]
        public async Task InsertUserOk()
        {
            var userDummie = new UserRequestDummie().Generate();

            var userResponse = new UserResponseDummie(userDummie).Generate();

            _userService.InsertUserAsync(Arg.Any<UserRequest>()).Returns(userResponse);
            var result = (ObjectResult) await _userController.InsertUser(userDummie);

            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            await _userService.Received().InsertUserAsync(Arg.Any<UserRequest>());
        }

        [Fact]
        public async Task UpdateUserOk()
        {
            var user = new UserUpdateRequestDummie().Generate();
            var userResponse = new UserResponseDummie(user).Generate();

            _userService.UpdatedUserAsync(Arg.Any<UserUpdatedRequest>()).Returns(userResponse);
            var result = (ObjectResult) await _userController.UpdateUser(user);

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            await _userService.Received().UpdatedUserAsync(Arg.Any<UserUpdatedRequest>());
        }
    }
}