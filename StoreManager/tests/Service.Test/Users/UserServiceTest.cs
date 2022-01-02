using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Authentication;
using System.Threading.Tasks;
using Application.Users;
using AutoMapper;
using Bogus;
using Core.Auth.Models;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Mappings;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Service.Test.Users;

public class UserServiceTest
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IRoleRepository _roleRepository;

    public UserServiceTest()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _roleRepository = Substitute.For<IRoleRepository>();

        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserMappingProfile());
            cfg.AddProfile(new RoleMappingProfile());
        });

        var mapper = mockMapper.CreateMapper();
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", "ASDasdasda"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        _userService = new UserService(_userRepository, _roleRepository, mapper, _configuration);
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
        _userRepository.CreateUser(Arg.Any<User>()).Returns(user);

        var result = await _userService.InsertUserAsync(userRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().CreateUser(Arg.Any<User>());
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
        _userRepository.CreateUser(Arg.Any<User>()).Returns(user);

        await Assert.ThrowsAsync<Exception>(() => _userService.InsertUserAsync(userRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUser(Arg.Any<User>());
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
        _userRepository.UpdateUser(Arg.Any<User>()).Returns(user);

        var result = await _userService.UpdatedUserAsync(userUpdateRequest);

        result.Should().NotBeNull();
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.Received().UpdateUser(Arg.Any<User>());
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
        _userRepository.UpdateUser(Arg.Any<User>()).Returns(user);

        await Assert.ThrowsAsync<Exception>(() => _userService.UpdatedUserAsync(userUpdateRequest));
        await _roleRepository.Received().CheckIfRoleExist(Arg.Any<int>());
        await _userRepository.DidNotReceive().CreateUser(Arg.Any<User>());
    }

    [Fact]
    public async Task GetUserOk()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUser(Arg.Any<int>()).Returns(user);

        var result = await _userService.GetUserAsync(user.Id);

        result.Should().NotBeNull();
        await _userRepository.Received().GetUser(Arg.Any<int>());
    }

    [Fact]
    public async Task GetUsersOk()
    {
        var users = new UserDummie().Generate(new Random().Next(1, 100));
        _userRepository.GetUsers().Returns(users);

        var result = await _userService.GetUsersAsync();

        result.Should().NotBeNull();
        await _userRepository.Received().GetUsers();
    }

    [Fact]
    public async Task GetUserByEmailAndPasswordOk()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUserByEmailAndPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(user);

        var authLoginRequest = new AuthLoginRequest
        {
            Email = user.Email,
            Password = user.Password
        };

        var result = await _userService.GetUserByEmailAndPasswordAsync(authLoginRequest);

        result.Password.Should().BeEmpty();
        await _userRepository.Received().GetUserByEmailAndPassword(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetUserByEmailAndPasswordException()
    {
        var user = new UserDummie().Generate();
        _userRepository.GetUserByEmailAndPassword(Arg.Any<string>(), Arg.Any<string>()).ReturnsNull();

        var authLoginRequest = new AuthLoginRequest
        {
            Email = user.Email,
            Password = user.Password
        };

        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _userService.GetUserByEmailAndPasswordAsync(authLoginRequest));
        
        await _userRepository.Received().GetUserByEmailAndPassword(Arg.Any<string>(), Arg.Any<string>());
    }
}