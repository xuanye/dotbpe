
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpRequestParsePostPlugin : IHttpPlugin
    {
        Task<(StatusCode requestStatusCode, string errorMessage)> ParseAsync(HttpRequest request, object requestMessage);
    }
}
