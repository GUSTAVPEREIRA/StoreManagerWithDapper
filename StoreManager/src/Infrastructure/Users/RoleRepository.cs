using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Configurations;
using Core.Users;
using Core.Users.Repositories;
using Dapper;
using Infrastructure.Providers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Users
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IConfiguration _configuration;
        private const string GetRoleQuery = @"SELECT id, name, is_admin FROM roles WHERE id = @id";
        private const string GetRolesQuery = @"SELECT id, name, is_admin FROM roles";

        private const string InsertRoleQuery =
            @"INSERT INTO roles (name, is_admin) VALUES (@name, @is_admin) RETURNING Id";

        private const string UpdateRoleQuery = @"UPDATE roles SET name = @name, is_admin = @is_admin WHERE id = @id";
        private const string DeleteRoleQuery = @"DELETE FROM roles WHERE id = @id";

        public RoleRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Role> GetRoleAsync(int id)
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PostgresConnectionProvider().GetConnection(connectionString);

            var role = await connection.QueryFirstOrDefaultAsync<Role>(GetRoleQuery, new
            {
                id
            });

            return role;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PostgresConnectionProvider().GetConnection(connectionString);

            var roles = await connection.QueryAsync<Role>(GetRolesQuery);

            return roles;
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PostgresConnectionProvider().GetConnection(connectionString);

            var roleId = await connection.ExecuteAsync(InsertRoleQuery, new
            {
                name = role.Name,
                is_admin = role.IsAdmin
            });

            role.Id = roleId;

            return role;
        }

        public async Task DeleteRoleAsync(int id)
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PostgresConnectionProvider().GetConnection(connectionString);

            await connection.ExecuteAsync(DeleteRoleQuery, new
            {
                id
            });
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PostgresConnectionProvider().GetConnection(connectionString);

            var isUpdated = await connection.ExecuteAsync(UpdateRoleQuery, new
            {
                name = role.Name,
                is_admin = role.IsAdmin,
                id = role.Id
            });
            
            return isUpdated == 1 ? role : null;
        }
    }
}