using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpRequestParsePlugin : IHttpPlugin
    {
        Task<(object, StatusCode, string)> ParseAsync(HttpRequest request);
    }
}
