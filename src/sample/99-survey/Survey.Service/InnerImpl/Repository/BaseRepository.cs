using Vulcan.DataAccess;

namespace Survey.Service.InnerImpl.Repository
{
    public class BaseRepository : Vulcan.DataAccess.ORMapping.MySql.MySqlRepository
    {
        private readonly string _constr;
        public BaseRepository(string constr) : base(constr)
        {
            this._constr = constr;
        }
        public TransScope GetTransScope(TransScopeOption option = TransScopeOption.Required)
        {
            return new TransScope(_constr, option);
        }
    }
}
