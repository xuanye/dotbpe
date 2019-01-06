using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    //格式化输出接口
    public interface IHttpOutputPlugin<in T>:IHttpPlugin where T: class
    {
        Task<bool> OutputAsync(HttpRequest req, HttpResponse res, T msg, HttpRouteOptions routeOption);
    }
}
