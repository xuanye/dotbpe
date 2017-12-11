using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.Plugin.Gateway
{
    public static class ApplicationBuilderExtension
    {

        public static IApplicationBuilder UseForwardProxy(this IApplicationBuilder builder)
        {
            builder.Run(Proccess);
            return builder;
        }
        private static Task Proccess(HttpContext context)
        {
            var service = context.RequestServices.GetRequiredService<IForwardService>();
            return ForwardHandler.Process(service, context);
        }

    }
}

