using System.Collections.Generic;
using Application.Auth;
using Core.Auth.Interfaces;
using Core.Errors;
using Dummie.Test.Users;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Service.Test.Users;

public class JwtServiceTest
{
    private readonly IJwtService _jwtService;

    public JwtServiceTest()
    {
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", "as4d6a4s65d1a2s0x48as1xasx98as1xa06x"},
            {"AuthSettings:JwtExpireTimesInMinuts", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();

        _jwtService = new JwtService(configuration);
    }

    [Fact]
    public void GenerateTokenWithClaimOk()
    {
        var authUser = new AuthUserResponseDummie().Generate();
        authUser.Role.IsAdmin = true;

        var result = _jwtService.GenerateToken(authUser);

        result.Should().NotBeNull();
    }
    
    [Fact]
    public void GenerateTokenWithoutClaimOk()
    {
        var authUser = new AuthUserResponseDummie().Generate();
        authUser.Role.IsAdmin = false;

        var result = _jwtService.GenerateToken(authUser);

        result.Should().NotBeNull();
    }
    
    [Fact]
    public void GenerateTokenWithExcpetion()
    {
        var myConfigurations = new Dictionary<string, string>
        {
            {"AuthSettings:JwtSecret", ""},
            {"AuthSettings:JwtExpireTimesInMinuts", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurations)
            .Build();
        
        var authUser = new AuthUserResponseDummie().Generate();
        var jwtService = new JwtService(configuration);

        Assert.Throws<EnvironmentVariableNotFoundException>(() => jwtService.GenerateToken(authUser));
    }
}