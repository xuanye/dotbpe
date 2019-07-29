using System.Threading.Tasks;
using Tomato.Rpc.BestPractice;
using Microsoft.AspNetCore.Http;

namespace Tomato.Gateway
{

    /// <summary>
    ///
    /// </summary>
    public interface IHttpParsePlugin: IHttpPlugin
    {
        /// <summary>
        /// Parse Http Request Data into dictData
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        /// <param name="dictData"></param>
        /// <param name="routeOption"></param>
        /// <returns></returns>
        Task<bool> ParseAsync(HttpRequest req, HttpResponse res, IHttpRequestData dictData,RouteItem routeItem);
    }

}
