using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc.Hosting
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRpcHostedService(this IServiceCollection services)
        {
            return services.AddHostedService<RpcHostedService>();
        }

    }
}
