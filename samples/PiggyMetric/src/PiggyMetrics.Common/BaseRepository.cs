using Vulcan.DataAccess;
using Vulcan.DataAccess.ORMapping.MySql;

namespace PiggyMetrics.Common
{
    public class BaseRepository:MySqlRepository
    {
        private readonly string _constr;
        public BaseRepository(string constr):base(constr){
            this._constr = constr;
        }
        public TransScope GetTransScope( TransScopeOption option = TransScopeOption.Required)
        {
            return new TransScope(_constr, option);
        }
    }
}
