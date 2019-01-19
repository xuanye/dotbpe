using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Logging;
using PiggyMetrics.AccountService.Repository;
using PiggyMetrics.Common;

namespace PiggyMetrics.AccountService.Impl
{
    public class AccountServiceImpl : AccountServiceBase
    {
        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<AccountServiceImpl>();
        private readonly AccountRepository _accountRep;
        public AccountServiceImpl(AccountRepository accountRep){
            this._accountRep = accountRep;
        }

        public override async Task<AccountRsp> CreateAsync(UserReq user)
        {
            AccountRsp rsp = new AccountRsp();
            try
            {
                Logger.Debug("receive CreateAsync,data="+Google.Protobuf.JsonFormatter.Default.Format(user));
                Account existing = await this._accountRep.FindByNameAsync(user.Account);

                Assert.IsNotNull(existing,"用户已经存在了");

                Logger.Debug("start call AuthService");
                //调用远端
                var authClient = ClientProxy.GetClient<AuthServiceClient>();
                var voidRsp = await authClient.CreateAsync(user);
                if(voidRsp.Status !=0){
                    rsp.Status = voidRsp.Status;
                    rsp.Message = voidRsp.Message;
                    return rsp;
                }
                Logger.Debug("end call AuthService");

                Saving saving = new Saving()
                {
                    Amount = 0,
                    Currency = Currency.Usd,
                    Interest = 0,
                    Deposit = false,
                    Capitalization = false,
                    Account = user.Account
                };
                Account account = new Account();

                account.Name = user.Account;
                account.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                account.LastSeenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Logger.Debug("start save to db");
                await this._accountRep.SaveUserAsync(account);
                await this._accountRep.SaveAccountSavingAsync(saving);

                Logger.Info("new account has been created:{0} " , user.Account);
                rsp.Data = account;
            }
            catch(Exception ex)
            {
                rsp.Status  = -1;
                rsp.Message = ex.Message;
            }
            return rsp;


        }

        public override async Task<AccountRsp> FindByNameAsync(FindAccountReq request)
        {
            AccountRsp rsp = new AccountRsp();
            try
            {

                Account account  = await this._accountRep.FindByNameAsync(request.Current);
                Assert.IsNull(account,"account not found");

                List<Item> incomes = await this._accountRep.FindIncomesAsync(account.Name);
                List<Item> expenses = await this._accountRep.FindExpensesAsync(account.Name);

                account.Saving = await this._accountRep.FindSavingAsync(account.Name);

                account.Incomes.Add(incomes);
                account.Expenses.Add(expenses);

                rsp.Data =account;
            }
            catch(Exception ex)
            {
                rsp.Status  = -1;
                rsp.Message = ex.Message;
            }
            return rsp;
        }

        public override async Task<VoidRsp> SaveAsync(AccountReq req)
        {
            VoidRsp rsp = new VoidRsp();
            //数据校验
            try
            {
                using(var scope = _accountRep.GetTransScope())
                {
                    await this._accountRep.UpdateUserInfoAsync(req);

                    req.Saving.Account = req.Name;

                    await this._accountRep.UpdateAccountSavingAsync(req.Saving);


                    await this._accountRep.DeleteIncomesAsync(req.Name);
                    await this._accountRep.DeleteExpensesAsync(req.Name);
                    await this._accountRep.AddIncomesAsync(req.Name,req.Incomes);
                    await this._accountRep.AddExpensesAsync(req.Name,req.Expenses);
                    Logger.Debug("SaveAsync Receieve Data:{0}",req.ToString());
                    //调用远端服务
                    var statClient = ClientProxy.GetClient<StatisticServiceClient>();
                    var statRsp = await statClient.UpdateStatisticsAsync(req);
                    if(statRsp.Status !=0){
                        rsp.Status = statRsp.Status;
                        rsp.Message = statRsp.Message;
                        return rsp;
                    }
                    scope.Complete();
                }



                Logger.Debug("add account stat success");
            }
            catch(Exception ex){
                rsp.Status  = -1;
                rsp.Message = ex.Message+ex.StackTrace;

                Logger.Error(ex,"save error:"+ex.Message+ex.StackTrace);
            }

            return new VoidRsp();

        }
    }
}
