using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Plugin.Gateway
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(HttpContext context, AuthenticateOption option);
        Task<LogoutResult> LogoutAsync(HttpContext context);
    }

    public class LoginResult: CallCommonResult
    {
  
        public string Account { get; set; }

        public override string ToString()
        {
            string body = string.Format("\"status\":{0},\"message\":\"{1}\",\"account\":\"{2}\"", Status, Message, Account);

            return "{" + body + "}";
        }
    }
    public class LogoutResult: CallCommonResult
    {
       
    }
}
