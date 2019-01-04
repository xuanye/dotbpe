using System;
using System.Threading.Tasks;
using DotBPE.Rpc.Logging;
using PiggyMetrics.AuthService.Repository;
using PiggyMetrics.Common;


namespace PiggyMetrics.AuthService.Impl
{
    public class AuthServiceImpl:AuthServiceBase
    {
        static readonly ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<AuthServiceImpl>();
        private readonly AuthRepository _repo;
        public AuthServiceImpl(AuthRepository repo)
        {
            this._repo = repo;
        }
        public override async Task<VoidRsp> CreateAsync(UserReq user)
        {
            VoidRsp rsp = new VoidRsp();
            rsp.Status = 0;
            rsp.Message = "test";
            try
            {
                Logger.Debug("receive CreateAsync,data="+Google.Protobuf.JsonFormatter.Default.Format(user));

                UserReq existing = await this._repo.FindByNameAsync(user.Account);
                if (existing !=null)
                {
                    Logger.Debug("user already exists:{0}", user.Account);
                }
                Assert.IsNotNull(existing, "user already exists:"+ user.Account);

                user.Password = CryptographyManager.Md5Encrypt(user.Account + "$" + user.Password);
                Logger.Debug("saving db");
                await this._repo.SaveUserAsync(user);
                Logger.Debug("saving db success");
            }
            catch(Exception ex){
                rsp.Status = -1;
                rsp.Message = ex.Message;
            }

            return rsp;
        }

        public override async Task<AuthRsp> AuthAsync(UserReq user)
        {
             var rsp = new AuthRsp();

            try
            {
                UserReq existing = await this._repo.FindByNameAsync(user.Account);

                if(existing == null)
                {
                    rsp.Status = -1;
                    rsp.Message = "account not found!";
                    return rsp;
                }

                string  enpass = CryptographyManager.Md5Encrypt(user.Account + "$" + user.Password);

                if (enpass == existing.Password)
                {
                    await this._repo.UpdateLastSenTimeAsync(user.Account,DateTime.Now);
                    rsp.Status = 0;
                    rsp.Account = user.Account;
                }
                else
                {
                    rsp.Status = -1;
                    rsp.Message = "wrong account/password";
                }
            }
            catch(Exception ex)
            {
                rsp.Status = -1;
                rsp.Message = ex.Message;
                Logger.Error("auth error:" + ex.Message + ex.StackTrace);
            }

            return rsp;
        }
    }
}
