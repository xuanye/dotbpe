using System.Threading.Tasks;
using DotBPE.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    //格式化输出接口
    public interface IHttpOutputPlugin:IHttpPlugin
    {
        Task<bool> OutputAsync(HttpRequest req, HttpResponse res, IJsonResult msg, RouteItem routeItem);
    }
}
