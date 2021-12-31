using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Auth.Models;
using Core.Configurations.Extensions;
using Core.Cryptography;
using Core.Errors;
using Core.Users;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.Extensions.Configuration;

namespace Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserResponse> InsertUserAsync(UserRequest userRequest)
        {
            await CheckRoleExists(userRequest.RoleId);
            var user = _mapper.Map<User>(userRequest);

            user.Disabled = false;
            user.Password = EncryptPassword(user.Password);
            user = await _userRepository.CreateUser(user);

            return _mapper.Map<UserResponse>(user);
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

        public async Task<UserResponse> UpdatedUserAsync(UserUpdatedRequest updatedRequest)
        {
            await CheckRoleExists(updatedRequest.RoleId);
            var user = _mapper.Map<User>(updatedRequest);

            user = await _userRepository.UpdateUser(user);

            return _mapper.Map<UserResponse>(user);
        }

        private async Task CheckRoleExists(int roleId)
        {
            if (await _roleRepository.CheckIfRoleExist(roleId) == false)
            {
                throw new Exception("Role does not exist!");
            }
        }

        public async Task<UserResponse> GetUserAsync(int id)
        {
            var user = await _userRepository.GetUser(id);

            var userResponse = _mapper.Map<UserResponse>(user);

            return userResponse;
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsers();
            var usersResponse = _mapper.Map<IEnumerable<UserResponse>>(users);

            return usersResponse;
        }

        public async Task<AuthUserResponse> GetUserByEmailAndPasswordAsync(AuthLoginRequest loginRequest)
        {
            loginRequest.Password = EncryptPassword(loginRequest.Password);
            var user = await _userRepository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);
            var authUserResponse = _mapper.Map<AuthUserResponse>(user);

            return authUserResponse;
        }
    }
}