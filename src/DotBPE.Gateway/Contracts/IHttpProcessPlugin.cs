using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpProcessPlugin: IHttpPlugin
    {
        Task ProcessAsync(HttpRequest req, HttpResponse res);
    }
}
