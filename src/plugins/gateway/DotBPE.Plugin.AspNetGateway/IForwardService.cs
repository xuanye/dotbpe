using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Plugin.AspNetGateway
{
    public interface IForwardService
    {
        Task<RpcContentResult> ForwardAysnc(HttpContext context);
    }
}
