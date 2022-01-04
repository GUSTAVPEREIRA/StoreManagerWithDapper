using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using AutoMapper;
using Core.Auth.Models;
using Core.Configurations;
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

        public async Task<UserResponse> UpdatedUserAsync(UserUpdatedRequest updatedRequest)
        {
            await CheckRoleExists(updatedRequest.RoleId);
            var user = _mapper.Map<User>(updatedRequest);

            user = await _userRepository.UpdateUser(user);

            return _mapper.Map<UserResponse>(user);
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

            if (user == null)
            {
                throw new AuthenticationException();
            }

            user.Password = "";
            var authUserResponse = _mapper.Map<AuthUserResponse>(user);

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

            var user = await UpdateDefaultUser(settings) ?? await CreateDefaultUser(settings);
            user.Password = "";
            
            return _mapper.Map<UserResponse>(user);
        }

        private async Task<User> UpdateDefaultUser(Setting settings)
        {
            var user = await _userRepository.GetUserByEmail(settings.AuthSettings.DefaulUserEmail);

            if (user == null)
            {
                return null;
            }

            user.Password = EncryptPassword(settings.AuthSettings.DefaultUserPassword);
            await _userRepository.UpdateUser(user);
            await _userRepository.ChangeUserPassword(user);

            return user;
        }

        private async Task<User> CreateDefaultUser(Setting settings)
        {
            var user = new User
            {
                Disabled = false,
                Email = settings.AuthSettings.DefaulUserEmail,
                Password = EncryptPassword(settings.AuthSettings.DefaultUserPassword),
                Role = new Role
                {
                    Name = "Administrador",
                    IsAdmin = true
                },
                FullName = "Usuário administrador"
            };

            user.Role = await _roleRepository.CreateRoleAsync(user.Role);

            return await _userRepository.CreateUser(user);
        }
    }
}