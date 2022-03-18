using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Gateway
{
    public interface IHttpApiOutputProcess
    {
        Task<bool> ProcessAsync(HttpRequest request,HttpResponse response, Encoding encoding, RpcResult result);
    }
}
