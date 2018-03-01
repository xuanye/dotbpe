using DotBPE.Rpc;
using DotBPE.Rpc.Client;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Netty;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DotBPE.Protocol.Amp
{
    public class AmpClient
    {
        public static IRpcClient<AmpMessage> Create(string remoteAddress, int multiplexCount = 1,Action<IServiceCollection> configServices =null,Action<ILoggingBuilder> configLogging=null)
        {
            var client = new RpcClientBuilder()
                .AddCore<AmpMessage>()
                .UseSetting("MultiplexCount", multiplexCount.ToString())
                .UserNettyClient<AmpMessage>()
                .ConfigureServices((services) =>
                {
                    configServices?.Invoke(services);

                    if (configLogging != null)
                    {
                        services.AddLogging(configLogging);
                    }

                    services.AddSingleton<IMessageCodecs<AmpMessage>, AmpCodecs>();                  
                })               
                .UseServer(remoteAddress)
                .Build<AmpMessage>();

            return client;
        }
    }
}
