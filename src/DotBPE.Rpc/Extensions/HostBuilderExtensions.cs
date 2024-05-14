// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using DotBPE.Rpc.Protocols;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseRpcServer(this IHostBuilder builder, Action<RpcServerOptions> configServer = null)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddDotBPEServer(configServer);
            });
        }

        public static IHostBuilder BindService<TService>(this IHostBuilder builder) where TService : class, IServiceActor
        {
            return builder.ConfigureServices(services => services.BindService<TService>());
        }

        public static IHostBuilder BindServices<TClassInAssembly>(this IHostBuilder builder, Func<ServiceModel, bool> filterFunc = null)
        {
            return builder.ConfigureServices(services => services.BindServices(typeof(Assembly).Assembly, filterFunc));
        }

        public static async Task<IHost> RunServerAsync(this IHostBuilder builder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var host = builder.UseConsoleLifetime().Build();
            var actorBuilder = host.Services.GetRequiredService<IServiceActorBuilder>();
            actorBuilder.Build();
            await host.RunAsync(cancellationToken);
            return host;
        }
    }
}