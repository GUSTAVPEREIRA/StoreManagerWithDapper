using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Users.Interfaces;
using Core.Users.Models;
using Dapper;
using Infrastructure.Providers;
using Infrastructure.Users.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Users;

public class UserRepository : BaseRepository, IUserRepository
{
    private readonly IMapper _mapper;

    private const string CreateUserQuery =
        @"INSERT INTO users 
              (email, password, disabled, full_name, role_id) 
              VALUES 
              (@email, @password, @disabled, @full_name, @role_id) RETURNING id";

    private const string UpdateUserQuery = @"UPDATE users SET
                 email = @email, disabled = @disabled, full_name = @full_name, role_id = @role_id 
                 WHERE id = @id";

    private const string SelectUserQuery = @"SELECT * FROM users as us
                              INNER JOIN roles AS rl ON us.role_id = rl.id 
                              WHERE us.id=@id";

    private const string SelectUsersQuery = @"SELECT * FROM users as us
                              INNER JOIN roles AS rl ON us.role_id = rl.id
                              WHERE true";

    private const string SelectUserByPasswordAndEmail = @"
        SELECT us.role_id AS role_id, us.email, us.id, us.full_name, us.disabled, rl.*
        FROM users AS us
        INNER JOIN roles AS rl ON us.role_id = rl.id
        WHERE us.email = @email AND us.password = @password";

    private const string SelectUserByEmailQuery = @"SELECT * FROM users as us
                              INNER JOIN roles AS rl ON us.role_id = rl.id 
                              WHERE us.email = @email";

    private const string UpdateUserPasswordQuery = @"UPDATE users SET
                 password = @password
                 WHERE id = @id";

    public UserRepository(IConfiguration configuration, IDbConnectionProvider provider, IMapper mapper) : base(
        configuration,
        provider)
    {
        _mapper = mapper;
    }

    public async Task<UserResponse> CreateUserAsync(UserRequest userRequest)
    {
        await using var connection = GetConnection();
        var user = _mapper.Map<UserRequest, User>(userRequest);

        var id = await connection.ExecuteScalarAsync<int>(CreateUserQuery, new
        {
            email = user.Email,
            password = user.Password,
            full_name = user.FullName,
            role_id = user.Role.Id,
            disabled = user.Disabled
        });

        user.Id = id;

        return _mapper.Map<User, UserResponse>(user);
    }

    public async Task<UserResponse> UpdateUserAsync(UserUpdatedRequest userUpdatedRequest)
    {
        await using var connection = GetConnection();
        var user = _mapper.Map<UserUpdatedRequest, User>(userUpdatedRequest);

        await connection.ExecuteScalarAsync<int>(UpdateUserQuery, new
        {
            email = user.Email,
            full_name = user.FullName,
            role_id = user.Role.Id,
            disabled = user.Disabled,
            id = user.Id
        });

        return _mapper.Map<User, UserResponse>(user);
    }

    public async Task<UserResponse> GetUserAsync(int id)
    {
        await using var connection = GetConnection();

        var user = connection.QueryAsync<User, Role, User>(SelectUserQuery,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new
            {
                id
            }, splitOn: "role_id").Result.FirstOrDefault();

        return _mapper.Map<User, UserResponse>(user);
    }

    public async Task<UserResponse> GetUserByEmailAsync(string email)
    {
        await using var connection = GetConnection();

        var user = connection.QueryAsync<User, Role, User>(SelectUserByEmailQuery,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new
            {
                email
            }, splitOn: "role_id").Result.FirstOrDefault();

        return _mapper.Map<User, UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        await using var connection = GetConnection();

        var users = connection.QueryAsync<User, Role, User>(SelectUsersQuery,
            (user, role) =>
            {
                user.Role = role;
                return user;
            }, splitOn: "role_id").Result;

        return _mapper.Map<IEnumerable<User>, IEnumerable<UserResponse>>(users);
    }

    public async Task<AuthUserResponse> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        await using var connection = GetConnection();

        var users = connection.QueryAsync<User, Role, User>(SelectUserByPasswordAndEmail,
            (user, role) =>
            {
                user.Role = role;
                return user;
            }, new
            {
                email,
                password
            }, splitOn: "role_id, id").Result;

        var user = users.FirstOrDefault();

        return _mapper.Map<User, AuthUserResponse>(user);
    }

    public async Task<UserResponse> ChangeUserPasswordAsync(UserRequest userRequest)
    {
        await using var connection = GetConnection();

        var user = _mapper.Map<UserRequest, User>(userRequest);

        await connection.ExecuteScalarAsync<int>(UpdateUserPasswordQuery, new
        {
            password = user.Password,
            id = user.Id
        });

        return _mapper.Map<User, UserResponse>(user);
    }
}