using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
   

    public class LoginResult : RpcResult<UserInfo>
    {
    }

    public class UserInfo
    {
        public string Account { get; set; }
    }

    public class LogoutResult : RpcResult
    {
    }
}
