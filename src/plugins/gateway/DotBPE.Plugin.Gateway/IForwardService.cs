using DotBPE.Rpc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Plugin.Gateway
{
    public interface IForwardService
    {
        Task<CallContentResult> ForwardAysnc(HttpContext context);
    }
}
