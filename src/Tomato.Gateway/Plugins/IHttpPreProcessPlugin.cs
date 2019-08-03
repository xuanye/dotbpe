using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    public interface IHttpPreProcessPlugin : IHttpPlugin
    {
        Task<bool> PreProcessAsync(HttpRequest req,HttpResponse res, RouteItem routeItem);
    }
}
