using System;
using System.Threading.Tasks;
using Api.Auth;
using Bogus;
using Core.Auth.Interfaces;
using Core.Auth.Models;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Controller.Test.Auth;

public class AuthControllerTest
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _jwtService = Substitute.For<IJwtService>();
        _userService = Substitute.For<IUserService>();
        _authController = new AuthController(_userService, _jwtService);
    }

    [Fact]
    public async Task AuthLoginOk()
    {
        var response = new AuthUserResponseDummie().Generate();

        var authLoginRequest = new AuthLoginRequest
        {
            Email = response.Email,
            Password = response.Password
        };

        var bearerTokenResponse = new BearerTokenResponse
        {
            Token = new Faker().Lorem.Paragraph()
        };

        _userService.GetUserByEmailAndPasswordAsync(Arg.Any<AuthLoginRequest>()).Returns(response);
        _jwtService.GenerateToken(Arg.Any<AuthUserResponse>()).Returns(bearerTokenResponse);

        var result = (ObjectResult) await _authController.AuthLogin(authLoginRequest);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().NotBeNull();
        await _userService.Received().GetUserByEmailAndPasswordAsync(Arg.Any<AuthLoginRequest>());
        _jwtService.Received().GenerateToken(Arg.Any<AuthUserResponse>());
    }

    [Fact]
    public async Task AuthLoginUnauthorized()
    {
        var response = new AuthUserResponseDummie().Generate();

        var authLoginRequest = new AuthLoginRequest
        {
            Email = response.Email,
            Password = response.Password
        };

        var bearerTokenResponse = new BearerTokenResponse
        {
            Token = new Faker().Lorem.Paragraph()
        };

        _userService.GetUserByEmailAndPasswordAsync(Arg.Any<AuthLoginRequest>()).Throws<Exception>();
        _jwtService.GenerateToken(Arg.Any<AuthUserResponse>()).Returns(bearerTokenResponse);

        var result = (ObjectResult) await _authController.AuthLogin(authLoginRequest);

        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        await _userService.Received().GetUserByEmailAndPasswordAsync(Arg.Any<AuthLoginRequest>());
        _jwtService.DidNotReceive().GenerateToken(Arg.Any<AuthUserResponse>());
    }

    [Fact]
    public async Task RestartDefaultUserOk()
    {
        var user = new UserResponseDummie().Generate();
        _userService.CreateOrUpdateUserDefault().Returns(user);
        var result = (ObjectResult) await _authController.RestartDefaultUser();

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        await _userService.Received().CreateOrUpdateUserDefault();
    }
}