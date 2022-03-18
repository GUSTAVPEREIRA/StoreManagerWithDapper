using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Application.Users;
using Core.Auth.Models;
using Core.Errors;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Service.Test.Users;

public class UserServiceTest
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;
    private readonly IRoleRepository _roleRepository;

    public UserServiceTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _roleRepository = Substitute.For<IRoleRepository>();

        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", "ASDasdasda"},
            {"AuthSettings:DefaultUserPassword", "123456"},
            {"AuthSettings:DefaulUserEmail", "teste@hotmail.com"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        _userService = new UserService(_userRepository, _roleRepository, configuration);
    }

    [Fact]
    public async Task InsertUserOk()
    {
        var userResponse = new UserResponseDummie().Generate();
        var userRequest = new UserRequestDummie().Generate();

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(true);
        _userRepository.CreateUserAsync(Arg.Any<UserRequest>()).Returns(userResponse);

        var result = await _userService.InsertUserAsync(userRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().CreateUserAsync(Arg.Any<UserRequest>());
    }

    [Fact]
    public async Task InsertUserRoleNotFoundException()
    {
        var userRequest = new UserRequestDummie().Generate();
        var userResponse = new UserResponseDummie().Generate();

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(false);
        _userRepository.CreateUserAsync(Arg.Any<UserRequest>()).Returns(userResponse);

        await Assert.ThrowsAsync<Exception>(() => _userService.InsertUserAsync(userRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserRequest>());
    }

    [Fact]
    public async Task UpdateUserOk()
    {
        var userUpdateRequest = new UserUpdateRequestDummie().Generate();
        var userResponse = new UserResponseDummie().Generate();

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(true);
        _userRepository.UpdateUserAsync(Arg.Any<UserUpdatedRequest>()).Returns(userResponse);

        var result = await _userService.UpdatedUserAsync(userUpdateRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());
    }

    [Fact]
    public async Task UpdateUserRoleNotFoundException()
    {
        var userUpdateRequest = new UserUpdateRequestDummie().Generate();
        var userResponse = new UserResponseDummie().Generate();

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(false);
        _userRepository.UpdateUserAsync(Arg.Any<UserUpdatedRequest>()).Returns(userResponse);

        await Assert.ThrowsAsync<Exception>(() => _userService.UpdatedUserAsync(userUpdateRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserUpdatedRequest>());
    }

    [Fact]
    public async Task GetUserOk()
    {
        var userResponse = new UserResponseDummie().Generate();
        _userRepository.GetUserAsync(Arg.Any<int>()).Returns(userResponse);

        var result = await _userService.GetUserAsync(userResponse.Id);

        result.Should().NotBeNull();
        await _userRepository.Received().GetUserAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetUsersOk()
    {
        var userResponses = new UserResponseDummie().Generate(new Random().Next(1, 100));
        _userRepository.GetUsersAsync().Returns(userResponses);

        var result = await _userService.GetUsersAsync();

        result.Should().NotBeNull();
        await _userRepository.Received().GetUsersAsync();
    }

    [Fact]
    public async Task GetUserByEmailAndPasswordOk()
    {
        var authUserResponse = new AuthUserResponseDummie().Generate();
        _userRepository.GetUserByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(authUserResponse);

        var authLoginRequest = new AuthLoginRequest
        {
            Email = authUserResponse.Email,
            Password = authUserResponse.Password
        };

        var result = await _userService.GetUserByEmailAndPasswordAsync(authLoginRequest);

        result.Password.Should().BeEmpty();
        await _userRepository.Received().GetUserByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetUserByEmailAndPasswordException()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUserByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>()).ReturnsNull();

        var authLoginRequest = new AuthLoginRequest
        {
            Email = user.Email,
            Password = user.Password
        };

        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _userService.GetUserByEmailAndPasswordAsync(authLoginRequest));

        await _userRepository.Received().GetUserByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task CreateUserDefaultOk()
    {
        var roleResponse = new RoleResponseDummie().Generate();
        var userResponse = new UserResponseDummie().Generate();
        userResponse.Role = roleResponse;

        _roleRepository.CreateRoleAsync(Arg.Any<RoleRequest>()).Returns(roleResponse);
        _userRepository.CreateUserAsync(Arg.Any<UserRequest>()).Returns(userResponse);

        var result = await _userService.CreateOrUpdateUserDefault();

        await _roleRepository.Received().CreateRoleAsync(Arg.Any<RoleRequest>());
        await _userRepository.Received().CreateUserAsync(Arg.Any<UserRequest>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());

        userResponse.Password = "";
        result.Should().BeEquivalentTo(userResponse);
    }

    [Fact]
    public async Task CreateUserDefaultWithException()
    {
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", ""},
            {"AuthSettings:DefaultUserPassword", "123456"},
            {"AuthSettings:DefaulUserEmail", "asdhasdhua@gmail.com"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        var userService = new UserService(_userRepository, _roleRepository, configuration);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<RoleRequest>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserRequest>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());
    }

    [Fact]
    public async Task UpdateUserDefaultOk()
    {
        var user = new UserResponseDummie().Generate();

        _userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(user);
        _userRepository.ChangeUserPasswordAsync(Arg.Any<UserUpdatedRequest>()).Returns(user);
        _userRepository.UpdateUserAsync(Arg.Any<UserUpdatedRequest>()).Returns(user);

        var result = await _userService.CreateOrUpdateUserDefault();

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<RoleRequest>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserRequest>());
        await _userRepository.Received().GetUserByEmailAsync(Arg.Any<string>());
        await _userRepository.Received().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());
        await _userRepository.Received().ChangeUserPasswordAsync(Arg.Any<UserUpdatedRequest>());

        user.Password = "";
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task UpdateUserDefaultWithException()
    {
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", ""},
            {"AuthSettings:DefaultUserPassword", "123456"},
            {"AuthSettings:DefaulUserEmail", "asdhasdhua@gmail.com"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        var userService = new UserService(_userRepository, _roleRepository, configuration);
        var user = new UserResponseDummie().Generate();

        _userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(user);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _userRepository.Received().GetUserByEmailAsync(Arg.Any<string>());
        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<RoleRequest>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserRequest>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());
        await _userRepository.DidNotReceive().ChangeUserPasswordAsync(Arg.Any<UserUpdatedRequest>());
    }

    [Fact]
    public async Task CreateOrUpdateUserDefaultWithException()
    {
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", "ASDasdasda"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        var userService = new UserService(_userRepository, _roleRepository, configuration);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<RoleRequest>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<UserRequest>());
        await _userRepository.DidNotReceive().GetUserByEmailAsync(Arg.Any<string>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<UserUpdatedRequest>());
        await _userRepository.DidNotReceive().ChangeUserPasswordAsync(Arg.Any<UserUpdatedRequest>());
    }
}