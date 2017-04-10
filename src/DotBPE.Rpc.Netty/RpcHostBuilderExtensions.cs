using System;
using System.Collections.Generic;
using System.Text;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotBPE.Rpc.Netty
{
    public static class RpcHostBuilderExtensions
    {
        public static IRpcHostBuilder UseNettyServer<TMessage>(this IRpcHostBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<IServerBootstrap, NettyServerBootstrap<TMessage>>();
            });
            return builder;
        }
    }
}
