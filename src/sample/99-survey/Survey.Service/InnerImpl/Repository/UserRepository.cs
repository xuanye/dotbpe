using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Survey.Service.InnerImpl.Domain;
using System.Threading.Tasks;
using Vulcan.DataAccess;

namespace Survey.Service.InnerImpl.Repository
{
    public class UserRepository : BaseRepository
    {
      
        public UserRepository(IConnectionManagerFactory factory,IOptions<DBOption> Option, ILoggerFactory loggerFactory)
          : base(factory,Option.Value.Master, loggerFactory)
        {
        }
        /// <summary>
        /// 检查用户登录信息是否正确
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<int> CheckPassword(string account, string checkpass)
        {
            var user = await base.GetAsync<UserInfo>("select account,password from user_info where account=@Account", new { Account = account });

            if (user == null)
            {
                return -1;
            }

            if (checkpass != user.Password)
            {
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="account">账号信息</param>
        /// <returns></returns>
        public Task<UserInfo> GetUser(string account)
        {
            string sql = "select user_id,account,full_name,password,is_admin,create_time,update_time from user_info where account=@Account";
            return base.GetAsync<UserInfo>(sql, new { Account = account });
        }
    }
}
