using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(HttpContext context, AuthenticateOption option);
        Task<LogoutResult> LogoutAsync(HttpContext context);
    }

    public class LoginResult: CallCommonResult<UserInfo>
    {  
    }

    public class UserInfo
    {
        public string Account { get; set; }
    }

    public class LogoutResult: CallCommonResult
    {
        
    }
}
