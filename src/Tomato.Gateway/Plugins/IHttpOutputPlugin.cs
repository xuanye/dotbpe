using System.Threading.Tasks;
using Tomato.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    //格式化输出接口
    public interface IHttpOutputPlugin:IHttpPlugin
    {
        Task<bool> OutputAsync(HttpRequest req, HttpResponse res, IJsonResult msg, RouteItem routeItem);
    }
}
