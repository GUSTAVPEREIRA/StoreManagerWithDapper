using System.Data.Common;
using System.Data.SQLite;
using Infrastructure.Providers;

namespace Repository.Test.Configuration
{
    public class SqLiteDbConnectionProvider : IDbConnectionProvider
    {
        public DbConnection GetConnection(string connection)
        {
            return new SQLiteConnection(connection);
        }
    }
}