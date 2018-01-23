using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Survey.Core;
using System;
using System.Threading.Tasks;

namespace Survey.Service.GateImpl
{
    public class UserGateService : UserGateServiceBase
    {

        private readonly ClientProxy _proxy;
        private readonly ILoginService _loginService;
        public UserGateService(ClientProxy proxy,ILoginService loginService)
        {
            this._proxy = proxy;
            this._loginService = loginService;
        }

        public override Task<RpcResult<GetUserRsp>> CheckLoginAsync(CheckLoginReq request)
        {

            var result = new RpcResult<GetUserRsp>();

            if (string.IsNullOrEmpty(request.Identity))
            {
                result.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                return Task.FromResult(result);
            }

            GetUserReq getUserReq = new GetUserReq();
            getUserReq.ClientIp = request.ClientIp;
            getUserReq.Identity = request.Identity;
            getUserReq.XRequestId = request.XRequestId;
            getUserReq.UserId = request.Identity;

            var client = this._proxy.GetClient<UserInnerServiceClient>();

            return client.GetUserInfoAsync(getUserReq);
        }

        public override Task<RpcResult<EditUserRsp>> EditUserAsync(EditUserReq request)
        {
            var result = new RpcResult<EditUserRsp>();
          
            if (string.IsNullOrEmpty(request.Identity))
            {
                result.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                return Task.FromResult(result);
            }

            var client = this._proxy.GetClient<UserInnerServiceClient>();

            return client.EditAsync(request);
        }

        public override async Task<RpcResult<LoginRsp>> LoginAsync(LoginReq request)
        {
            var client = this._proxy.GetClient<UserInnerServiceClient>();

            var result = await client.LoginAsync(request);
            if (result.Code == 0 && result.Data !=null)
            {
                string sessionId = Guid.NewGuid().ToString("N");
                SessionUser user = new SessionUser();
                user.BpeSessionId = sessionId;
                user.LoginTimestamp = 1;
                user.Identity = result.Data.Account;

                await this._loginService.SetSessionAsync(user); //设置登录的Session信息到redis 中


                result.Data.BpeSessionId = sessionId; //登录成功 生成sessionId
            }
            return result;
        }

        public override Task<RpcResult<RegisterRsp>> RegisterAsync(RegisterReq request)
        {
            var client = this._proxy.GetClient<UserInnerServiceClient>();

            return client.RegisterAsync(request);
        }
    }
}
