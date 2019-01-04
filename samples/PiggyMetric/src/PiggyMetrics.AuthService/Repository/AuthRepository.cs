using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common;

namespace PiggyMetrics.AuthService.Repository
{
    public class AuthRepository:BaseRepository
    {
        public AuthRepository(IOptions<DbOption> dbOption) : base(dbOption.Value.AuthDbConStr)
        {
        }


        public Task<UserReq> FindByNameAsync(string account)
        {
            string sql = "select account as Account,password as Password from user_auth where account=@Account";
            return base.GetAsync<UserReq>(sql, new {Account = account});
        }

        public Task SaveUserAsync(UserReq user)
        {
            string sql =
                "INSERT INTO user_auth (`account`,`password`,`create_time`,`last_sen_time`) VALUES (@Account,@Password,@CreateTime,@LastSenTime)";

            return base.ExcuteAsync(sql,
                new
                {
                    user.Account,
                    user.Password,
                    CreateTime = DateTime.Now,
                    LastSenTime = DateTime.Now
                });

        }

        public Task UpdateLastSenTimeAsync(string account,DateTime now)
        {
            string sql =
                "UPDATE user_auth SET `last_sen_time`=@LastSenTime WHERE account=@Account";
            return base.ExcuteAsync(sql, new {Account = account, LastSenTime = now});

        }
    }
}
