using System.Threading.Tasks;
using Tomato.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    //用于处理HTTP响应发送前的信息
    public interface IHttpPostProcessPlugin:IHttpPlugin
    {
        Task<bool> PostProcessAsync(HttpRequest req, HttpResponse res,IJsonResult result, RouteItem routeItem);
    }
}
