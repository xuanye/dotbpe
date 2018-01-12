using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Survey.Service
{
    public class MySqlConnectionFactory : Vulcan.DataAccess.IConnectionFactory
    {
        public IDbConnection CreateDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
