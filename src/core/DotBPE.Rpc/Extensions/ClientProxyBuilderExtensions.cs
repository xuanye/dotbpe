using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Server;
using DotBPE.Rpc.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Rpc
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientProxyBuilderExtensions
    {
        /// <summary>
        /// 添加客户端所需核心的接口依赖
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IClientProxyBuilder AddCore<TMessage>(this IClientProxyBuilder builder) where TMessage : InvokeMessage
        {
            builder.ConfigureServices((services) =>
            {
                services.AddSingleton<ITransportFactory<TMessage>, DefaultTransportFactory<TMessage>>()
                    .AddSingleton<IClientMessageHandler<TMessage>, ClientMessageHandler<TMessage>>()
                    .AddSingleton<IRouter<TMessage>, TransforPolicyRouter<TMessage>>()
                    .AddSingleton<IServiceActorLocator<TMessage>, NoopServiceActorLocator<TMessage>>()
                .AddSingleton<IRpcClient<TMessage>, DefaultRpcClient<TMessage>>();
            });
            return builder;
        }


        /// <summary>
        /// Uses the server.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <returns></returns>
        public static IClientProxyBuilder UseServer(this IClientProxyBuilder builder, string remoteAddress)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), "服务器地址不能为空");
            builder.UseSetting("DefaultServerAddress", remoteAddress);
            return builder;
        }

        /// <summary>
        /// Uses the server.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <param name="multiplexCount">The multiplex count.</param>
        /// <returns></returns>
        public static IClientProxyBuilder UseServer(this IClientProxyBuilder builder, string remoteAddress, int multiplexCount)
        {
            Preconditions.CheckArgument(!string.IsNullOrEmpty(remoteAddress), "服务器地址不能为空");

            Preconditions.CheckArgument(multiplexCount <= 0, "链接数不能小于0");

            builder.UseSetting("DefaultServerAddress", remoteAddress);
            builder.UseSetting("MultiplexCount", multiplexCount.ToString());
            return builder;
        }

        /// <summary>
        /// Configures the logging services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureLogging">The configure logging.</param>
        /// <returns></returns>
        public static IClientProxyBuilder ConfigureLoggingServices(this IClientProxyBuilder builder, Action<ILoggingBuilder> configureLogging)
        {
            Preconditions.CheckNotNull(configureLogging, "configureLogging");
            return builder.ConfigureServices(services => services.AddLogging(configureLogging));
        }

    }
}
