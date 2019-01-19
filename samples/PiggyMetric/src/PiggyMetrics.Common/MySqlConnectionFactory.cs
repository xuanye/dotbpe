
using System.Data;
using MySql.Data.MySqlClient;

namespace PiggyMetrics.Common
{
    public class MySqlConnectionFactory : Vulcan.DataAccess.IConnectionFactory
    {
        public IDbConnection CreateDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
