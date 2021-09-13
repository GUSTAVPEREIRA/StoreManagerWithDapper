using System.Data.Common;
using Npgsql;

namespace Infrastructure.Providers
{
    public class PosgresConnectionProvider : IDbConnectionProvider
    {
        public DbConnection GetConnection(string connection)
        {
            return new NpgsqlConnection(connection);
        }
    }
}