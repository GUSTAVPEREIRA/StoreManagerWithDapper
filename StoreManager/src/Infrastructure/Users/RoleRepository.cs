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

        public RoleRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<Role> GetRoleAsync(int id)
        {
            var connectionString = _configuration.GetConnectionString();
            await using var connection = new PosgresConnectionProvider().GetConnection(connectionString);

            var role  = await connection.QueryFirstOrDefaultAsync<Role>(GetRoleQuery, new
            {
                id
            });

            return role;
        }

        public Task<List<Role>> GetRolesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Role> CreateRoleAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Role> DeleteRoleAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Role> UpdateRoleAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}