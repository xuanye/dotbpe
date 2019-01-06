using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    //用于处理HTTP响应发送前的信息
    public interface IHttpPostProcessPlugin<in T>:IHttpPlugin where T:class
    {
        Task<bool> PostProcessAsync(HttpRequest req, HttpResponse res,T msg, HttpRouteOptions routeOption);
    }
}
