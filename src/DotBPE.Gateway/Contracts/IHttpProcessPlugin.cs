// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpProcessPlugin : IHttpPlugin
    {
        Task<object> ProcessAsync(HttpRequest req, HttpResponse res);
    }
}
