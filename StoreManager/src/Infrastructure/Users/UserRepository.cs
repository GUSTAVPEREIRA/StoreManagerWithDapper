using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Users;
using Core.Users.Interfaces;
using Dapper;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Users
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private const string CreateUserQuery =
            @"INSERT INTO users 
              (email, password, disabled, full_name, role_id) 
              VALUES 
              (@email, @password, @disabled, @full_name, @role_id) RETURNING id";

        private const string UpdateUserQuery = @"UPDATE users SET
                 email=@email, disabled=@disabled, full_name=@full_name, role_id=@role_id 
                 WHERE id=@id";

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
                              WHERE us.email=@email";

        private const string UpdateUserPasswordQuery = @"UPDATE users SET
                 password=@password
                 WHERE id=@id";

        public UserRepository(IConfiguration configuration, IDbConnectionProvider provider) : base(configuration,
            provider)
        {
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await using var connection = GetConnection();

            var id = await connection.ExecuteScalarAsync<int>(CreateUserQuery, new
            {
                email = user.Email,
                password = user.Password,
                full_name = user.FullName,
                role_id = user.Role.Id,
                disabled = user.Disabled
            });

            user.Id = id;

            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            await using var connection = GetConnection();

            await connection.ExecuteScalarAsync<int>(UpdateUserQuery, new
            {
                email = user.Email,
                full_name = user.FullName,
                role_id = user.Role.Id,
                disabled = user.Disabled,
                id = user.Id
            });

            return user;
        }

        public async Task<User> GetUserAsync(int id)
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

            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
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

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            await using var connection = GetConnection();

            var users = connection.QueryAsync<User, Role, User>(SelectUsersQuery,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                }, splitOn: "role_id").Result;

            return users;
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
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

            return users.FirstOrDefault();
        }

        public async Task<User> ChangeUserPasswordAsync(User user)
        {
            await using var connection = GetConnection();

            await connection.ExecuteScalarAsync<int>(UpdateUserPasswordQuery, new
            {
                password = user.Password,
                id = user.Id
            });

            return user;
        }
    }
}