using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tomato.Gateway
{
    public class GatewayMiddleware
    {
        private readonly IServiceProvider _provider;
        private readonly RequestDelegate _next;

        private readonly IProtocolProcessor _processor;
        public GatewayMiddleware(IServiceProvider provider,RequestDelegate next)
        {
            this._provider = provider;
            this._next = next;

            this._processor = this._provider.GetRequiredService<IProtocolProcessor>();

        }


        public async Task Invoke(HttpContext context)
        {
            var invoked = await this._processor.Invoke(context);
            if (invoked)
            {
                return;
            }
            await this._next(context);
        }
    }
}
