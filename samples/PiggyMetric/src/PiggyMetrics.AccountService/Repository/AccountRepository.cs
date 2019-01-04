using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common;

namespace PiggyMetrics.AccountService.Repository
{
    public class AccountRepository:BaseRepository
    {
        public AccountRepository(IOptions<DbOption> option):base(option.Value.AccountDbConStr){

        }

        internal Task SaveUserAsync(Account account)
        {
            string sql ="INSERT INTO `user_info` (`account`,`last_seen_time`,`create_time`,`note`)	VALUES	(@Name,@LastSeenTime,@CreateTime,@Note)";
            return base.ExcuteAsync(sql,account);
        }

        internal  Task<Account> FindByNameAsync(string account)
        {
            string sql ="SELECT `account` as Name,DATE_FORMAT(`last_seen_time`,'%Y-%m-%d %H:%i:%s') as LastSeenTime,DATE_FORMAT(`create_time`,'%Y-%m-%d %H:%i:%s') as CreateTime,`note` as Note FROM `user_info` where account=@Account ";

            return base.GetAsync<Account>(sql,new {Account = account});
        }

        internal Task<List<Item>> FindIncomesAsync(string account)
        {
            string sql ="SELECT `title` as Title,`amount` as Amount,`currency` as Currency,`period` as Period,`icon` as Icon FROM `user_income` where account=@Account";
            return base.QueryAsync<Item>(sql,new {Account = account});
        }

        internal Task<List<Item>> FindExpensesAsync(string account)
        {
            string sql ="SELECT `title` as Title,`amount` as Amount,`currency` as Currency,`period` as Period,`icon` as Icon FROM `user_expense` where account=@Account";
            return base.QueryAsync<Item>(sql,new {Account = account});
        }

        internal Task<Saving> FindSavingAsync(string account)
        {
            string sql = "SELECT `amount`,`currency`,`interest`,`deposit`,`capitalization`	FROM `user_saving` WHERE account=@Account";

            return base.GetAsync<Saving>(sql,new {Account = account});
        }

        internal Task SaveAccountSavingAsync(Saving saving)
        {
            string sql = @"INSERT INTO `user_saving`
                            (`account`,`amount`,`currency`,`interest`,`deposit`,`capitalization`)
                        VALUES
                            (@Account,@Amount,@Currency,@Interest,@Deposit,@Capitalization)";

            return base.ExcuteAsync(sql,saving);
        }

        internal Task UpdateUserInfoAsync(AccountReq account)
        {
            string sql ="UPDATE `user_info` SET `last_seen_time`=now(),`note`=@Note WHERE account=@Name";

            return base.ExcuteAsync(sql,account);
        }

        internal Task DeleteIncomesAsync(string account)
        {
            string sql = "DELETE FROM `user_income` WHERE account=@Account";
            return base.ExcuteAsync(sql,new {Account =account});
        }

        internal Task DeleteExpensesAsync(string account)
        {
            string sql = "DELETE FROM `user_expense` WHERE account=@Account";
            return base.ExcuteAsync(sql,new {Account =account});
        }

        internal Task UpdateAccountSavingAsync(Saving saving)
        {
            string sql = @"UPDATE `user_saving`
                            SET `amount`=@Amount,`currency`=@Currency,
                            `interest`=@Interest,`deposit`=@Deposit,`capitalization`=@Capitalization
                        WHERE account=@Account";

            return base.ExcuteAsync(sql,saving);
        }

        internal async Task AddIncomesAsync(string account,IList<Item> incomes)
        {
           string sql =@"INSERT INTO `user_income`
            (`account`,`title`,`amount`,`currency`,`period`,`icon`)
        VALUES
            (@Account,@Title,@Amount,@Currency,@Period,@Icon)";


            foreach(var item in incomes){
               item.Account =account;
               await base.ExcuteAsync(sql,item);
            }

        }

        internal async Task AddExpensesAsync(string account,IList<Item> expenses)
        {
            string sql =@"INSERT INTO `user_expense`
            (`account`,`title`,`amount`,`currency`,`period`,`icon`)
        VALUES
            (@Account,@Title,@Amount,@Currency,@Period,@Icon)";

            foreach(var item in expenses){
                item.Account =account;
                await base.ExcuteAsync(sql,item);
            }
        }
    }
}
