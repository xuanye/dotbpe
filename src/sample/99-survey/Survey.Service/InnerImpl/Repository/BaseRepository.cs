using Microsoft.Extensions.Logging;
using Vulcan.DataAccess;

namespace Survey.Service.InnerImpl.Repository
{
    public class BaseRepository : Vulcan.DataAccess.ORMapping.MySql.MySqlRepository
    {
        private readonly string _constr;
        private static ILogger<SQLMetrics> _logger; 
        public BaseRepository(IConnectionManagerFactory factory,string constr,ILoggerFactory loggerFactory) : base(factory,constr)
        {
            this._constr = constr;
            if(_logger == null)
            {
                _logger = loggerFactory.CreateLogger<SQLMetrics>();
            }
        }

        protected override ISQLMetrics CreateSQLMetrics()
        {
            return new SQLMetrics(_logger);
        }
    }
}
