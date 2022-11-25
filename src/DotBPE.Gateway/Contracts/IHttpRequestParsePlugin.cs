// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpRequestParsePlugin : IHttpPlugin
    {
        Task<(object, int, string)> ParseAsync(HttpRequest request);
    }
}
