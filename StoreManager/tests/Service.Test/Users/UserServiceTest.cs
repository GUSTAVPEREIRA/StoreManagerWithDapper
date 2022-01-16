using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Application.Users;
using AutoMapper;
using Bogus;
using Core.Auth.Models;
using Core.Errors;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Mappings;
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
    private readonly IMapper _mapper;

    public UserServiceTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _roleRepository = Substitute.For<IRoleRepository>();

        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserMappingProfile());
            cfg.AddProfile(new RoleMappingProfile());
        });

        _mapper = mockMapper.CreateMapper();
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", "ASDasdasda"},
            {"AuthSettings:DefaultUserPassword", "123456"},
            {"AuthSettings:DefaulUserEmail", "teste@hotmail.com"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        _userService = new UserService(_userRepository, _roleRepository, _mapper, configuration);
    }

    [Fact]
    public async Task InsertUserOk()
    {
        var userRequest = new UserRequestDummie().Generate();
        var user = new User
        {
            Disabled = true,
            Email = userRequest.Email,
            Id = new Faker().Random.Int(1, 9999),
            Password = userRequest.Password,
            Role = new Role
            {
                Id = userRequest.RoleId,
                Name = new Faker().Random.String(),
                IsAdmin = new Faker().Random.Bool()
            },
            FullName = userRequest.FullName
        };

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(true);
        _userRepository.CreateUserAsync(Arg.Any<User>()).Returns(user);

        var result = await _userService.InsertUserAsync(userRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().CreateUserAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task InsertUserRoleNotFoundException()
    {
        var userRequest = new UserRequestDummie().Generate();
        var user = new User
        {
            Disabled = true,
            Email = userRequest.Email,
            Id = new Faker().Random.Int(1, 9999),
            Password = userRequest.Password,
            Role = new Role
            {
                Id = userRequest.RoleId,
                Name = new Faker().Random.String(),
                IsAdmin = new Faker().Random.Bool()
            },
            FullName = userRequest.FullName
        };

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(false);
        _userRepository.CreateUserAsync(Arg.Any<User>()).Returns(user);

        await Assert.ThrowsAsync<Exception>(() => _userService.InsertUserAsync(userRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task UpdateUserOk()
    {
        var userUpdateRequest = new UserUpdateRequestDummie().Generate();
        var user = new User
        {
            Disabled = true,
            Email = userUpdateRequest.Email,
            Id = userUpdateRequest.Id,
            Password = userUpdateRequest.Password,
            Role = new Role
            {
                Id = userUpdateRequest.RoleId,
                Name = new Faker().Random.String(),
                IsAdmin = new Faker().Random.Bool()
            },
            FullName = userUpdateRequest.FullName
        };

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(true);
        _userRepository.UpdateUserAsync(Arg.Any<User>()).Returns(user);

        var result = await _userService.UpdatedUserAsync(userUpdateRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().UpdateUserAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task UpdateUserRoleNotFoundException()
    {
        var userUpdateRequest = new UserUpdateRequestDummie().Generate();
        var user = new User
        {
            Disabled = true,
            Email = userUpdateRequest.Email,
            Id = userUpdateRequest.Id,
            Password = userUpdateRequest.Password,
            Role = new Role
            {
                Id = userUpdateRequest.RoleId,
                Name = new Faker().Random.String(),
                IsAdmin = new Faker().Random.Bool()
            },
            FullName = userUpdateRequest.FullName
        };

        _roleRepository.CheckIfRoleExist(Arg.Any<int>()).Returns(false);
        _userRepository.UpdateUserAsync(Arg.Any<User>()).Returns(user);

        await Assert.ThrowsAsync<Exception>(() => _userService.UpdatedUserAsync(userUpdateRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task GetUserOk()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUserAsync(Arg.Any<int>()).Returns(user);

        var result = await _userService.GetUserAsync(user.Id);

        result.Should().NotBeNull();
        await _userRepository.Received().GetUserAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetUsersOk()
    {
        var users = new UserDummie().Generate(new Random().Next(1, 100));
        _userRepository.GetUsersAsync().Returns(users);

        var result = await _userService.GetUsersAsync();

        result.Should().NotBeNull();
        await _userRepository.Received().GetUsersAsync();
    }

    [Fact]
    public async Task GetUserByEmailAndPasswordOk()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUserByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(user);

        var authLoginRequest = new AuthLoginRequest
        {
            Email = user.Email,
            Password = user.Password
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
        var role = new RoleDummie().Generate();
        var user = new UserDummie(role).Generate();
        var expectedResult = _mapper.Map<UserResponse>(user);

        _roleRepository.CreateRoleAsync(Arg.Any<Role>()).Returns(role);
        _userRepository.CreateUserAsync(Arg.Any<User>()).Returns(user);

        var result = await _userService.CreateOrUpdateUserDefault();

        await _roleRepository.Received().CreateRoleAsync(Arg.Any<Role>());
        await _userRepository.Received().CreateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>());

        expectedResult.Password = "";
        result.Should().BeEquivalentTo(expectedResult);
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

        var userService = new UserService(_userRepository, _roleRepository, _mapper, configuration);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<Role>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task UpdateUserDefaultOk()
    {
        var role = new RoleDummie().Generate();
        var user = new UserDummie(role).Generate();
        var expectedResult = _mapper.Map<UserResponse>(user);

        _userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(user);
        _userRepository.ChangeUserPasswordAsync(Arg.Any<User>()).Returns(user);
        _userRepository.UpdateUserAsync(Arg.Any<User>()).Returns(user);

        var result = await _userService.CreateOrUpdateUserDefault();

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<Role>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
        await _userRepository.Received().GetUserByEmailAsync(Arg.Any<string>());
        await _userRepository.Received().UpdateUserAsync(Arg.Any<User>());
        await _userRepository.Received().ChangeUserPasswordAsync(Arg.Any<User>());

        expectedResult.Password = "";
        result.Should().BeEquivalentTo(expectedResult);
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

        var userService = new UserService(_userRepository, _roleRepository, _mapper, configuration);
        var role = new RoleDummie().Generate();
        var user = new UserDummie(role).Generate();

        _userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(user);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _userRepository.Received().GetUserByEmailAsync(Arg.Any<string>());
        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<Role>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().ChangeUserPasswordAsync(Arg.Any<User>());
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

        var userService = new UserService(_userRepository, _roleRepository, _mapper, configuration);

        await Assert.ThrowsAsync<EnvironmentVariableNotFoundException>(() => userService.CreateOrUpdateUserDefault());

        await _roleRepository.DidNotReceive().CreateRoleAsync(Arg.Any<Role>());
        await _userRepository.DidNotReceive().CreateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().GetUserByEmailAsync(Arg.Any<string>());
        await _userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>());
        await _userRepository.DidNotReceive().ChangeUserPasswordAsync(Arg.Any<User>());
    }
}