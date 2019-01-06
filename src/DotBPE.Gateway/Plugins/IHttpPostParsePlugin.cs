using System.Threading.Tasks;
using DotBPE.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace DotBPE.Gateway
{
    public interface IHttpPostParsePlugin : IHttpPlugin
    {
        Task<bool> PostParseAsync(HttpRequest req, HttpResponse res, IHttpRequestData dictData, HttpRouteOptions routeOption);
    }

}
