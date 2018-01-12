using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using Survey.Core;
using System;
using System.Threading.Tasks;

namespace Survey.Service.GateImpl
{
    public class UserGateService : UserGateServiceBase
    {
        public override Task<RpcResult<EditUserRsp>> EditUserAsync(EditUserReq request)
        {
            var result = new RpcResult<EditUserRsp>();
          
            if (string.IsNullOrEmpty(request.Identity))
            {
                result.Code = ErrorCodes.AUTHORIZATION_REQUIRED;
                return Task.FromResult(result);
            }

            var client = ClientProxy.GetClient<UserInnerServiceClient>();
            return client.EditAsync(request);
        }

        public override async Task<RpcResult<LoginRsp>> LoginAsync(LoginReq request)
        {
            var client = ClientProxy.GetClient<UserInnerServiceClient>();

            var result = await client.LoginAsync(request);
            if (result.Code == 0 && result.Data !=null)
            {
                result.Data.SessionId = Guid.NewGuid().ToString("N"); //登录成功 生成sessionId
            }
            return result;
        }

        public override Task<RpcResult<RegisterRsp>> RegisterAsync(RegisterReq request)
        {
            var client = ClientProxy.GetClient<UserInnerServiceClient>();
            return client.RegisterAsync(request);
        }
    }
}
