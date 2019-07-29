using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomato.Gateway
{
    public interface IAdditionalHttpParser
    {
        /// <summary>
        /// Parse Http Request Data into dictData
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        /// <param name="dictData"></param>
        /// <param name="routeOption"></param>
        /// <returns></returns>
        Task<bool> ParseAsync(HttpRequest req, HttpResponse res, IHttpRequestData dictData, RouteItem routeItem);
    }
}
