using System.Threading.Tasks;
using Tomato.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{
    public interface IHttpPostParsePlugin : IHttpPlugin
    {
        Task<bool> PostParseAsync(HttpRequest req, HttpResponse res, IHttpRequestData dictData, RouteItem routeItem);
    }

}
