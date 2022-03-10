using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Core.Auth.Models;
using Core.Configurations;
using Core.Configurations.Extensions;
using Core.Cryptography;
using Core.Errors;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.Extensions.Configuration;

namespace Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    public async Task<UserResponse> InsertUserAsync(UserRequest userRequest)
    {
        userRequest.Password = EncryptPassword(userRequest.Password);

        return await _userRepository.CreateUserAsync(userRequest);
    }

    public async Task<UserResponse> UpdatedUserAsync(UserUpdatedRequest userUpdatedRequest)
    {
        await CheckRoleExists(userUpdatedRequest.RoleId);
        
        var userResponse = await _userRepository.UpdateUserAsync(userUpdatedRequest);
        userResponse.Role = await _roleRepository.GetRoleAsync(userResponse.Role.Id);
        
        return userResponse;
    }

    public async Task<UserResponse> GetUserAsync(int id)
    {
        return await _userRepository.GetUserAsync(id);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<AuthUserResponse> GetUserByEmailAndPasswordAsync(AuthLoginRequest loginRequest)
    {
        loginRequest.Password = EncryptPassword(loginRequest.Password);
        var authUserResponse =
            await _userRepository.GetUserByEmailAndPasswordAsync(loginRequest.Email, loginRequest.Password);

        if (authUserResponse == null)
        {
            throw new AuthenticationException();
        }

        authUserResponse.Password = "";

        return authUserResponse;
    }

    private async Task CheckRoleExists(int roleId)
    {
        if (await _roleRepository.CheckIfRoleExist(roleId) == false)
        {
            throw new Exception("Role does not exist!");
        }
    }

    private string EncryptPassword(string password)
    {
        var settings = _configuration.GetSettings();

        if (settings.AuthSettings == null || string.IsNullOrEmpty(settings.AuthSettings.JwtSecret))
        {
            throw new EnvironmentVariableNotFoundException(nameof(settings.AuthSettings.JwtSecret));
        }

        return Aes256.EncryptString(password, settings.AuthSettings.JwtSecret);
    }

    public async Task<UserResponse> CreateOrUpdateUserDefault()
    {
        var settings = _configuration.GetSettings();

        if (string.IsNullOrEmpty(settings.AuthSettings.DefaultUserPassword) ||
            string.IsNullOrEmpty(settings.AuthSettings.DefaulUserEmail))
        {
            throw new EnvironmentVariableNotFoundException(
                $"{nameof(settings.AuthSettings.DefaultUserPassword)} and {nameof(settings.AuthSettings.DefaulUserEmail)}");
        }

        var userResponse = await UpdateDefaultUser(settings) ?? await CreateDefaultUser(settings);
        userResponse.Password = "";

        return userResponse;
    }

    private async Task<UserResponse> UpdateDefaultUser(Setting settings)
    {
        var userResponse = await _userRepository.GetUserByEmailAsync(settings.AuthSettings.DefaulUserEmail);

        if (userResponse == null)
        {
            return null;
        }

        userResponse.Password = EncryptPassword(settings.AuthSettings.DefaultUserPassword);

        var userUpdatedRequest = new UserUpdatedRequest()
        {
            Id = userResponse.Id,
            Disabled = userResponse.Disabled,
            Email = userResponse.Email,
            Password = userResponse.Password,
            FullName = userResponse.FullName,
            RoleId = userResponse.Role.Id
        };

        await _userRepository.UpdateUserAsync(userUpdatedRequest);

        return await _userRepository.ChangeUserPasswordAsync(userUpdatedRequest);
    }

    private async Task<UserResponse> CreateDefaultUser(Setting settings)
    {
        var user = new UserRequest
        {
            Email = settings.AuthSettings.DefaulUserEmail,
            Password = EncryptPassword(settings.AuthSettings.DefaultUserPassword),
            FullName = "Usuário administrador"
        };

        var roleRequest = new RoleRequest()
        {
            Name = "Administrador",
            IsAdmin = true
        };

        var roleResponse = await _roleRepository.CreateRoleAsync(roleRequest);
        user.RoleId = roleResponse.Id;

        return await _userRepository.CreateUserAsync(user);
    }
}