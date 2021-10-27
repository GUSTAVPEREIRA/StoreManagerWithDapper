using Npgsql;
using System.Data.Common;

namespace Infrastructure.Providers
{
    public class PostgresConnectionProvider : IDbConnectionProvider
    {
        public DbConnection GetDbConnection(string connection)
        {
            return new NpgsqlConnection(connection);
        }
    }
}