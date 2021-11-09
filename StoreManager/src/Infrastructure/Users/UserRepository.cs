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
                 email=@email, password=@password, disabled=@disabled, full_name=@full_name, role_id=@role_id 
                 WHERE id=@id";

        private const string SelectUserQuery = @"SELECT * FROM users as us
                              INNER JOIN roles as rl ON us.role_id = rl.id 
                              WHERE us.id=@id";

        private const string SelectUsersQuery = @"SELECT * FROM users as us
                              INNER JOIN roles as rl ON us.role_id = rl.id
                              WHERE true";

        public UserRepository(IConfiguration configuration, IDbConnectionProvider provider) : base(configuration,
            provider)
        {
        }

        public async Task<User> CreateUser(User user)
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

        public async Task<User> UpdateUser(User user)
        {
            await using var connection = GetConnection();
            
            await connection.ExecuteScalarAsync<int>(UpdateUserQuery, new
            {
                email = user.Email,
                password = user.Password,
                full_name = user.FullName,
                role_id = user.Role.Id,
                disabled = user.Disabled,
                id = user.Id
            });

            return user;
        }

        public async Task<User> GetUser(int id)
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

        public async Task<IEnumerable<User>> GetUsers()
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
    }
}